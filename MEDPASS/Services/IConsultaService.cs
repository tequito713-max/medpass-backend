using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IConsultaService
    {
        Task<List<Consulta>> DameTodasLasConsultas();
        Task<Consulta?> DameConsultaPorId(int id);
        Task<Consulta> CrearConsulta(Consulta nuevaConsulta);
        Task<Consulta?> ActualizarConsulta(int id, Consulta consultaActualizar);
        Task<Consulta?> EliminarConsulta(int id);
    }
}