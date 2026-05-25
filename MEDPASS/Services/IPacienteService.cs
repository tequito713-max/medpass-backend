using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IPacienteService
    {
        Task<List<Paciente>> DameTodosLosPacientes();
        Task<Paciente?> DamePacientePorId(int id);
        Task<Paciente> CrearPaciente(Paciente nuevoPaciente);
        Task<Paciente?> ActualizarPaciente(int id, Paciente pacienteActualizar);
        Task<Paciente?> EliminarPaciente(int id);
    }
}