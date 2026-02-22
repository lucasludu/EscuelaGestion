using System.Collections.Generic;

namespace EscuelaGestion.Models
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación para las calificaciones
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
