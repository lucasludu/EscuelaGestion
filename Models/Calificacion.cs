namespace EscuelaGestion.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int CursoId { get; set; }
        public int MateriaId { get; set; }
        public int Trimestre { get; set; } // 1, 2, 3 o Final
        public string? Nota { get; set; }
        public int? Asistencia { get; set; }
        public string? Observaciones { get; set; }

        public Alumno? Alumno { get; set; }
        public Curso? Curso { get; set; }
        public Materia? Materia { get; set; }
    }
}
