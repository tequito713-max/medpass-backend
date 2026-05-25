using MEDPASS.Models;
using Microsoft.Data.SqlClient;
using System.Data;


namespace MEDPASS.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly string _connectionString;

        public RecetaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Receta>> DameTodasLasRecetas()
        {
            var lista = new List<Receta>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerRecetas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Receta
                {
                    Id = reader.GetInt32("Id"),
                    Medicamento = reader.GetString("Medicamento"),
                    Dosis = reader.GetString("Dosis"),
                    Duracion = reader.GetString("Duracion"),
                    ConsultaId = reader.GetInt32("ConsultaId")
                });
            }

            return lista;
        }

        public async Task<Receta?> DameRecetaPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerRecetaPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Receta
                {
                    Id = reader.GetInt32("Id"),
                    Medicamento = reader.GetString("Medicamento"),
                    Dosis = reader.GetString("Dosis"),
                    Duracion = reader.GetString("Duracion"),
                    ConsultaId = reader.GetInt32("ConsultaId")
                };
            }

            return null;
        }

        public async Task<Receta> CrearReceta(Receta nuevaReceta)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearReceta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Medicamento", nuevaReceta.Medicamento ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Dosis", nuevaReceta.Dosis ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Duracion", nuevaReceta.Duracion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ConsultaId", nuevaReceta.ConsultaId);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevaReceta.Id = Convert.ToInt32(id);
            return nuevaReceta;
        }

        public async Task<Receta?> ActualizarReceta(int id, Receta recetaActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarReceta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Medicamento", recetaActualizar.Medicamento ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Dosis", recetaActualizar.Dosis ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Duracion", recetaActualizar.Duracion ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? recetaActualizar : null;
        }

        public async Task<Receta?> EliminarReceta(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarReceta", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Receta { Id = id } : null;
        }
    }
}