using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
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
                SELECT Id, Nombre, Email, Rol, PacienteId, MedicoId
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

            string token = GenerarToken(id, nombre, email, rol, pacienteId, medicoId);

            return Ok(new
            {
                ok = true,
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
}
