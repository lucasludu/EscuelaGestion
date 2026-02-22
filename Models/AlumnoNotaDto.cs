using System.Collections.Generic;
using EscuelaGestion.ViewModels;

namespace EscuelaGestion.Models
{
    public class AlumnoNotaDto : ViewModelBase
    {
        private int? _asistencias;
        private string _observaciones = string.Empty;

        public int AlumnoId { get; set; }
        public string ApellidoNombre { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Grado { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        
        // Diccionario para almacenar las notas: <MateriaId, Nota>
        public Dictionary<int, decimal?> NotasPorMateria { get; set; } = new();

        public int? Asistencias 
        { 
            get => _asistencias; 
            set => SetProperty(ref _asistencias, value); 
        }

        public string Observaciones 
        { 
            get => _observaciones; 
            set => SetProperty(ref _observaciones, value); 
        }
    }
}
