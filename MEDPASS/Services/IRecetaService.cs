using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IRecetaService
    {
        Task<List<Receta>> DameTodasLasRecetas();
        Task<Receta?> DameRecetaPorId(int id);
        Task<Receta> CrearReceta(Receta nuevaReceta);
        Task<Receta?> ActualizarReceta(int id, Receta recetaActualizar);
        Task<Receta?> EliminarReceta(int id);
    }
}