namespace MEDPASS.Models
{
    public class Estudio
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int PacienteId { get; set; }
        public int ClinicaId { get; set; }
    }
}