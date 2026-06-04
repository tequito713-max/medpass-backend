using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MEDPASS.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    ok = false,
                    mensaje = "El correo y la contraseña son obligatorios."
                });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(@"
                SELECT Id, Nombre, Email, Rol, PacienteId, MedicoId, MfaSecret, MfaEnabled
                FROM Usuarios
                WHERE Email = @Email AND Password = @Password
            ", connection);

            command.Parameters.AddWithValue("@Email", request.Email);
            command.Parameters.AddWithValue("@Password", request.Password);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!reader.Read())
            {
                return Unauthorized(new
                {
                    ok = false,
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            int id = reader.GetInt32(reader.GetOrdinal("Id"));
            string nombre = reader.GetString(reader.GetOrdinal("Nombre"));
            string email = reader.GetString(reader.GetOrdinal("Email"));
            string rol = reader.GetString(reader.GetOrdinal("Rol"));

            int? pacienteId = reader.IsDBNull(reader.GetOrdinal("PacienteId"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("PacienteId"));

            int? medicoId = reader.IsDBNull(reader.GetOrdinal("MedicoId"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("MedicoId"));

            bool mfaEnabled = reader.GetBoolean(reader.GetOrdinal("MfaEnabled"));

            if (mfaEnabled)
            {
                return Ok(new
                {
                    ok = true,
                    requiereMfa = true,
                    mensaje = "Credenciales correctas. Se requiere código MFA.",
                    usuarioId = id,
                    email,
                    rol
                });
            }

            string token = GenerarToken(id, nombre, email, rol, pacienteId, medicoId);

            return Ok(new
            {
                ok = true,
                requiereMfa = false,
                mensaje = "Inicio de sesión correcto.",
                token,
                usuario = new
                {
                    id,
                    nombre,
                    email,
                    rol,
                    pacienteId,
                    medicoId
                }
            });
        }

        [HttpPost("configurar-mfa")]
        public async Task<IActionResult> ConfigurarMfa([FromBody] ConfigurarMfaRequest request)
        {
            if (request.UsuarioId <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    mensaje = "El usuarioId es obligatorio."
                });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            string? email = null;
            string? nombre = null;

            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using (SqlCommand buscarUsuario = new SqlCommand(@"
                SELECT Nombre, Email
                FROM Usuarios
                WHERE Id = @UsuarioId
            ", connection))
            {
                buscarUsuario.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);

                using SqlDataReader reader = await buscarUsuario.ExecuteReaderAsync();

                if (!reader.Read())
                {
                    return NotFound(new
                    {
                        ok = false,
                        mensaje = "Usuario no encontrado."
                    });
                }

                nombre = reader.GetString(reader.GetOrdinal("Nombre"));
                email = reader.GetString(reader.GetOrdinal("Email"));
            }

            byte[] secretBytes = KeyGeneration.GenerateRandomKey(20);
            string secretBase32 = Base32Encoding.ToString(secretBytes);

            using (SqlCommand actualizar = new SqlCommand(@"
                UPDATE Usuarios
                SET MfaSecret = @MfaSecret,
                    MfaEnabled = 0
                WHERE Id = @UsuarioId
            ", connection))
            {
                actualizar.Parameters.AddWithValue("@MfaSecret", secretBase32);
                actualizar.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);
                await actualizar.ExecuteNonQueryAsync();
            }

            string issuer = "MEDPASS";
            string label = $"{issuer}:{email}";
            string otpauthUrl = $"otpauth://totp/{Uri.EscapeDataString(label)}?secret={secretBase32}&issuer={Uri.EscapeDataString(issuer)}&digits=6&period=30";

            return Ok(new
            {
                ok = true,
                mensaje = "MFA generado. Registra la clave en Google Authenticator o Microsoft Authenticator.",
                usuarioId = request.UsuarioId,
                nombre,
                email,
                secret = secretBase32,
                otpauthUrl
            });
        }

        [HttpPost("activar-mfa")]
        public async Task<IActionResult> ActivarMfa([FromBody] VerificarMfaRequest request)
        {
            var usuario = await ObtenerUsuarioPorId(request.UsuarioId);

            if (usuario == null)
            {
                return NotFound(new
                {
                    ok = false,
                    mensaje = "Usuario no encontrado."
                });
            }

            if (string.IsNullOrWhiteSpace(usuario.MfaSecret))
            {
                return BadRequest(new
                {
                    ok = false,
                    mensaje = "El usuario todavía no tiene MFA configurado."
                });
            }

            bool codigoCorrecto = ValidarCodigoMfa(usuario.MfaSecret, request.Codigo);

            if (!codigoCorrecto)
            {
                return Unauthorized(new
                {
                    ok = false,
                    mensaje = "Código MFA incorrecto."
                });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(@"
                UPDATE Usuarios
                SET MfaEnabled = 1
                WHERE Id = @UsuarioId
            ", connection);

            command.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "MFA activado correctamente."
            });
        }

        [HttpPost("verificar-mfa")]
        public async Task<IActionResult> VerificarMfa([FromBody] VerificarMfaRequest request)
        {
            var usuario = await ObtenerUsuarioPorId(request.UsuarioId);

            if (usuario == null)
            {
                return NotFound(new
                {
                    ok = false,
                    mensaje = "Usuario no encontrado."
                });
            }

            if (!usuario.MfaEnabled || string.IsNullOrWhiteSpace(usuario.MfaSecret))
            {
                return BadRequest(new
                {
                    ok = false,
                    mensaje = "El usuario no tiene MFA activado."
                });
            }

            bool codigoCorrecto = ValidarCodigoMfa(usuario.MfaSecret, request.Codigo);

            if (!codigoCorrecto)
            {
                return Unauthorized(new
                {
                    ok = false,
                    mensaje = "Código MFA incorrecto."
                });
            }

            string token = GenerarToken(
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol,
                usuario.PacienteId,
                usuario.MedicoId
            );

            return Ok(new
            {
                ok = true,
                mensaje = "MFA validado correctamente.",
                token,
                usuario = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    email = usuario.Email,
                    rol = usuario.Rol,
                    pacienteId = usuario.PacienteId,
                    medicoId = usuario.MedicoId
                }
            });
        }

        private bool ValidarCodigoMfa(string secret, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            byte[] secretBytes = Base32Encoding.ToBytes(secret);
            Totp totp = new Totp(secretBytes);

            return totp.VerifyTotp(
                codigo,
                out long _,
                new VerificationWindow(previous: 1, future: 1)
            );
        }

        private async Task<UsuarioMfa?> ObtenerUsuarioPorId(int usuarioId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(@"
                SELECT Id, Nombre, Email, Rol, PacienteId, MedicoId, MfaSecret, MfaEnabled
                FROM Usuarios
                WHERE Id = @UsuarioId
            ", connection);

            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!reader.Read())
                return null;

            return new UsuarioMfa
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Rol = reader.GetString(reader.GetOrdinal("Rol")),
                PacienteId = reader.IsDBNull(reader.GetOrdinal("PacienteId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("PacienteId")),
                MedicoId = reader.IsDBNull(reader.GetOrdinal("MedicoId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("MedicoId")),
                MfaSecret = reader.IsDBNull(reader.GetOrdinal("MfaSecret"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("MfaSecret")),
                MfaEnabled = reader.GetBoolean(reader.GetOrdinal("MfaEnabled"))
            };
        }

        private string GenerarToken(int id, string nombre, string email, string rol, int? pacienteId, int? medicoId)
        {
            var jwtKey = _configuration["Jwt:Key"]!;
            var jwtIssuer = _configuration["Jwt:Issuer"]!;
            var jwtAudience = _configuration["Jwt:Audience"]!;
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, nombre),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim("rol", rol)
            };

            if (pacienteId.HasValue)
            {
                claims.Add(new Claim("pacienteId", pacienteId.Value.ToString()));
            }

            if (medicoId.HasValue)
            {
                claims.Add(new Claim("medicoId", medicoId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ConfigurarMfaRequest
    {
        public int UsuarioId { get; set; }
    }

    public class VerificarMfaRequest
    {
        public int UsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }

    public class UsuarioMfa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? PacienteId { get; set; }
        public int? MedicoId { get; set; }
        public string? MfaSecret { get; set; }
        public bool MfaEnabled { get; set; }
    }
}
