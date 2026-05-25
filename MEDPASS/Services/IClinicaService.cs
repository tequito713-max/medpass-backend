using MEDPASS.Models;

namespace MEDPASS.Services
{
    public interface IClinicaService
    {
        Task<List<Clinica>> DameTodasLasClinicas();
        Task<Clinica?> DameClinicaPorId(int id);
        Task<Clinica> CrearClinica(Clinica nuevaClinica);
        Task<Clinica?> ActualizarClinica(int id, Clinica clinicaActualizar);
        Task<Clinica?> EliminarClinica(int id);
    }
}