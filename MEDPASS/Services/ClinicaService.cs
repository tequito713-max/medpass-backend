using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class ClinicaService : IClinicaService
    {
        private readonly string _connectionString;

        public ClinicaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Clinica>> DameTodasLasClinicas()
        {
            var lista = new List<Clinica>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerClinicas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Clinica
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Direccion = reader.GetString("Direccion"),
                    Telefono = reader.GetString("Telefono"),
                    Codigo = reader.GetString("Codigo")
                });
            }

            return lista;
        }

        public async Task<Clinica?> DameClinicaPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerClinicaPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Clinica
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Direccion = reader.GetString("Direccion"),
                    Telefono = reader.GetString("Telefono"),
                    Codigo = reader.GetString("Codigo")
                };
            }

            return null;
        }

        public async Task<Clinica> CrearClinica(Clinica nuevaClinica)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearClinica", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Nombre", nuevaClinica.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", nuevaClinica.Direccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", nuevaClinica.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Codigo", nuevaClinica.Codigo ?? (object)DBNull.Value);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevaClinica.Id = Convert.ToInt32(id);
            return nuevaClinica;
        }

        public async Task<Clinica?> ActualizarClinica(int id, Clinica clinicaActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarClinica", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", clinicaActualizar.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", clinicaActualizar.Direccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", clinicaActualizar.Telefono ?? (object)DBNull.Value);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? clinicaActualizar : null;
        }

        public async Task<Clinica?> EliminarClinica(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarClinica", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Clinica { Id = id } : null;
        }
    }
}