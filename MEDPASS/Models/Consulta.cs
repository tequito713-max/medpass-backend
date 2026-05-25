namespace MEDPASS.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
        public string Diagnostico { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public int ClinicaId { get; set; }
    }
}