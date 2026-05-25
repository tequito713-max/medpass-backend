using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IMedicoService
    {
        Task<List<Medico>> DameTodosLosMedicos();
        Task<Medico?> DameMedicoPorId(int id);
        Task<Medico> CrearMedico(Medico nuevoMedico);
        Task<Medico?> ActualizarMedico(int id, Medico medicoActualizar);
        Task<Medico?> EliminarMedico(int id);
    }
}