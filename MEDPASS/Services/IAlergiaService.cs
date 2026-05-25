using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IAlergiaService
    {
        Task<List<Alergia>> DameTodasLasAlergias();
        Task<Alergia?> DameAlergiaPorId(int id);
        Task<Alergia> CrearAlergia(Alergia nuevaAlergia);
        Task<Alergia?> ActualizarAlergia(int id, Alergia alergiaActualizar);
        Task<Alergia?> EliminarAlergia(int id);
    }
}