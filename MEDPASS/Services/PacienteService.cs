using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly string _connectionString;

        public PacienteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Paciente>> DameTodosLosPacientes()
        {
            var lista = new List<Paciente>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerPacientes", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Paciente
                {
                    Id = reader.GetInt32("Id"),
                    CURP = reader.GetString("CURP"),
                    Nombre = reader.GetString("Nombre"),
                    FechaNac = reader.GetDateTime("FechaNac"),
                    TipoSangre = reader.GetString("TipoSangre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                });
            }

            return lista;
        }

        public async Task<Paciente?> DamePacientePorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerPacientePorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Paciente
                {
                    Id = reader.GetInt32("Id"),
                    CURP = reader.GetString("CURP"),
                    Nombre = reader.GetString("Nombre"),
                    FechaNac = reader.GetDateTime("FechaNac"),
                    TipoSangre = reader.GetString("TipoSangre"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                };
            }

            return null;
        }

        public async Task<Paciente> CrearPaciente(Paciente nuevoPaciente)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearPaciente", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CURP", nuevoPaciente.CURP ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Nombre", nuevoPaciente.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaNac", nuevoPaciente.FechaNac);
            cmd.Parameters.AddWithValue("@TipoSangre", nuevoPaciente.TipoSangre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", nuevoPaciente.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", nuevoPaciente.Email ?? (object)DBNull.Value);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevoPaciente.Id = Convert.ToInt32(id);
            return nuevoPaciente;
        }

        public async Task<Paciente?> ActualizarPaciente(int id, Paciente pacienteActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarPaciente", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", pacienteActualizar.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoSangre", pacienteActualizar.TipoSangre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", pacienteActualizar.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", pacienteActualizar.Email ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? pacienteActualizar : null;
        }

        public async Task<Paciente?> EliminarPaciente(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarPaciente", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Paciente { Id = id } : null;
        }
    }
}