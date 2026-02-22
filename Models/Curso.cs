using System.Collections.Generic;

namespace EscuelaGestion.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string NombreGrado { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public int AnioLectivo { get; set; }

        public string FullCourseName => $"{NombreGrado} - {Division}";
        
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
    }
}
