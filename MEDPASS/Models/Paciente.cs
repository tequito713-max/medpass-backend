namespace MEDPASS.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public string CURP { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNac { get; set; }
        public string TipoSangre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}