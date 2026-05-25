using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class EstudioService : IEstudioService
    {
        private readonly string _connectionString;

        public EstudioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Estudio>> DameTodosLosEstudios()
        {
            var lista = new List<Estudio>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerEstudios", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Estudio
                {
                    Id = reader.GetInt32("Id"),
                    Tipo = reader.GetString("Tipo"),
                    Resultado = reader.GetString("Resultado"),
                    Fecha = reader.GetDateTime("Fecha"),
                    PacienteId = reader.GetInt32("PacienteId"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                });
            }

            return lista;
        }

        public async Task<Estudio?> DameEstudioPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerEstudioPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Estudio
                {
                    Id = reader.GetInt32("Id"),
                    Tipo = reader.GetString("Tipo"),
                    Resultado = reader.GetString("Resultado"),
                    Fecha = reader.GetDateTime("Fecha"),
                    PacienteId = reader.GetInt32("PacienteId"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                };
            }

            return null;
        }

        public async Task<Estudio> CrearEstudio(Estudio nuevoEstudio)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearEstudio", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Tipo", nuevoEstudio.Tipo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Resultado", nuevoEstudio.Resultado ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", nuevoEstudio.Fecha);
            cmd.Parameters.AddWithValue("@PacienteId", nuevoEstudio.PacienteId);
            cmd.Parameters.AddWithValue("@ClinicaId", nuevoEstudio.ClinicaId);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevoEstudio.Id = Convert.ToInt32(id);
            return nuevoEstudio;
        }

        public async Task<Estudio?> ActualizarEstudio(int id, Estudio estudioActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarEstudio", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Tipo", estudioActualizar.Tipo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Resultado", estudioActualizar.Resultado ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? estudioActualizar : null;
        }

        public async Task<Estudio?> EliminarEstudio(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarEstudio", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Estudio { Id = id } : null;
        }
    }
}