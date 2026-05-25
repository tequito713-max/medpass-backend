using Microsoft.Data.SqlClient;
using System.Data;
using MEDPASS.Models;

namespace MEDPASS.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly string _connectionString;

        public MedicoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Medico>> DameTodosLosMedicos()
        {
            var lista = new List<Medico>();

            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerMedicos", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Medico
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Especialidad = reader.GetString("Especialidad"),
                    Cedula = reader.GetString("Cedula"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                });
            }

            return lista;
        }

        public async Task<Medico?> DameMedicoPorId(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ObtenerMedicoPorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Medico
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Especialidad = reader.GetString("Especialidad"),
                    Cedula = reader.GetString("Cedula"),
                    ClinicaId = reader.GetInt32("ClinicaId")
                };
            }

            return null;
        }

        public async Task<Medico> CrearMedico(Medico nuevoMedico)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CrearMedico", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Nombre", nuevoMedico.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Especialidad", nuevoMedico.Especialidad ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Cedula", nuevoMedico.Cedula ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ClinicaId", nuevoMedico.ClinicaId);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();

            nuevoMedico.Id = Convert.ToInt32(id);
            return nuevoMedico;
        }

        public async Task<Medico?> ActualizarMedico(int id, Medico medicoActualizar)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_ActualizarMedico", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", medicoActualizar.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Especialidad", medicoActualizar.Especialidad ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ClinicaId", medicoActualizar.ClinicaId);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? medicoActualizar : null;
        }

        public async Task<Medico?> EliminarMedico(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_EliminarMedico", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int filas = await cmd.ExecuteNonQueryAsync();

            return filas > 0 ? new Medico { Id = id } : null;
        }
    }
}