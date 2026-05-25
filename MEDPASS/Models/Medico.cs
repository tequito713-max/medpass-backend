namespace MEDPASS.Models
{
    public class Medico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public string Cedula { get; set; }
        public int ClinicaId { get; set; }
    }
}