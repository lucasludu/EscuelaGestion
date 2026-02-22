namespace EscuelaGestion.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        public string ApellidoNombre { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public int? CursoId { get; set; }
        
        public virtual Curso? Curso { get; set; }
    }
}
