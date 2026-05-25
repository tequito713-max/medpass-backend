using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly string _connectionString;

        public ConsultaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Consulta>> DameTodasLasConsultas()
        {
            var lista = new List<Consulta>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerConsultas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Consulta
                {
                    Id = reader.GetInt32("Id"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Motivo = reader.GetString("Motivo"),
                    Diagnostico = reader.GetString("Diagnostico"),
                    PacienteId = reader.GetInt32("PacienteId"),
                    MedicoId = reader.GetInt32("MedicoId"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                });
            }

            return lista;
        }

        public async Task<Consulta?> DameConsultaPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerConsultaPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Consulta
                {
                    Id = reader.GetInt32("Id"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Motivo = reader.GetString("Motivo"),
                    Diagnostico = reader.GetString("Diagnostico"),
                    PacienteId = reader.GetInt32("PacienteId"),
                    MedicoId = reader.GetInt32("MedicoId"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                };
            }

            return null;
        }

        public async Task<Consulta> CrearConsulta(Consulta nuevaConsulta)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearConsulta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Fecha", nuevaConsulta.Fecha);
            cmd.Parameters.AddWithValue("@Motivo", nuevaConsulta.Motivo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Diagnostico", nuevaConsulta.Diagnostico ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PacienteId", nuevaConsulta.PacienteId);
            cmd.Parameters.AddWithValue("@MedicoId", nuevaConsulta.MedicoId);
            cmd.Parameters.AddWithValue("@ClinicaId", nuevaConsulta.ClinicaId);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevaConsulta.Id = Convert.ToInt32(id);
            return nuevaConsulta;
        }

        public async Task<Consulta?> ActualizarConsulta(int id, Consulta consultaActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarConsulta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Motivo", consultaActualizar.Motivo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Diagnostico", consultaActualizar.Diagnostico ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? consultaActualizar : null;
        }

        public async Task<Consulta?> EliminarConsulta(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarConsulta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Consulta { Id = id } : null;
        }
    }
}