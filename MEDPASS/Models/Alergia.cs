namespace MEDPASS.Models
{
    public class Alergia
    {
        public int Id { get; set; }
        public string Sustancia { get; set; } = string.Empty;
        public string Reaccion { get; set; } = string.Empty;
        public string Severidad { get; set; } = string.Empty;
        public int PacienteId { get; set; }
    }
}