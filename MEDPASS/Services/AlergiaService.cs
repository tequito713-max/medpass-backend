using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class AlergiaService : IAlergiaService
    {
        private readonly string _connectionString;

        public AlergiaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Alergia>> DameTodasLasAlergias()
        {
            var lista = new List<Alergia>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerAlergias", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Alergia
                {
                    Id = reader.GetInt32("Id"),
                    Sustancia = reader.GetString("Sustancia"),
                    Reaccion = reader.GetString("Reaccion"),
                    Severidad = reader.GetString("Severidad"),
                    PacienteId = reader.GetInt32("PacienteId")
                });
            }

            return lista;
        }

        public async Task<Alergia?> DameAlergiaPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerAlergiaPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Alergia
                {
                    Id = reader.GetInt32("Id"),
                    Sustancia = reader.GetString("Sustancia"),
                    Reaccion = reader.GetString("Reaccion"),
                    Severidad = reader.GetString("Severidad"),
                    PacienteId = reader.GetInt32("PacienteId")
                };
            }

            return null;
        }

        public async Task<Alergia> CrearAlergia(Alergia nuevaAlergia)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearAlergia", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Sustancia", nuevaAlergia.Sustancia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Reaccion", nuevaAlergia.Reaccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Severidad", nuevaAlergia.Severidad ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PacienteId", nuevaAlergia.PacienteId);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevaAlergia.Id = Convert.ToInt32(id);
            return nuevaAlergia;
        }

        public async Task<Alergia?> ActualizarAlergia(int id, Alergia alergiaActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarAlergia", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Sustancia", alergiaActualizar.Sustancia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Reaccion", alergiaActualizar.Reaccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Severidad", alergiaActualizar.Severidad ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? alergiaActualizar : null;
        }

        public async Task<Alergia?> EliminarAlergia(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarAlergia", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Alergia { Id = id } : null;
        }
    }
}