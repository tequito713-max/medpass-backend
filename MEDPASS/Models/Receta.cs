namespace MEDPASS.Models
{
    public class Receta
    {
        public int Id { get; set; }
        public string Medicamento { get; set; }
        public string Dosis { get; set; }
        public string Duracion { get; set; }
        public int ConsultaId { get; set; }
    }
}