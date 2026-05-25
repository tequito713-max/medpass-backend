using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IEstudioService
    {
        Task<List<Estudio>> DameTodosLosEstudios();
        Task<Estudio?> DameEstudioPorId(int id);
        Task<Estudio> CrearEstudio(Estudio nuevoEstudio);
        Task<Estudio?> ActualizarEstudio(int id, Estudio estudioActualizar);
        Task<Estudio?> EliminarEstudio(int id);
    }
}