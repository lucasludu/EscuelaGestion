using EscuelaGestion.Helpers;
using EscuelaGestion.Models;
using EscuelaGestion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EscuelaGestion.ViewModels
{
    public class CalificacionesViewModel : ViewModelBase
    {
        private readonly EscuelaContext _context;
        private readonly IExcelService _excelService;
        
        private ObservableCollection<AlumnoNotaDto> _alumnosNotas = new();
        private ObservableCollection<Curso> _cursos = new();
        private List<Materia> _materiasActuales = new();
        private Curso? _selectedCurso;
        private int _trimestreSeleccionado = 1;

        public CalificacionesViewModel()
        {
            _context = new EscuelaContext();
            _excelService = new ExcelService();
            
            RefreshAll();
            
            GuardarCambiosCommand = new RelayCommand(_ => GuardarCambios());
            ExportarExcelCommand = new RelayCommand(_ => ExportarExcel(), _ => SelectedCurso != null);

            MessageHub.ConfigurationChanged += () => RefreshAll();
        }

        public void RefreshAll()
        {
            LoadMaterias();
            LoadCursos();
        }

        public ObservableCollection<AlumnoNotaDto> AlumnosNotas
        {
            get => _alumnosNotas;
            set => SetProperty(ref _alumnosNotas, value);
        }

        public ObservableCollection<Curso> Cursos
        {
            get => _cursos;
            set => SetProperty(ref _cursos, value);
        }

        public List<Materia> MateriasActuales => _materiasActuales;

        public Curso? SelectedCurso
        {
            get => _selectedCurso;
            set
            {
                if (SetProperty(ref _selectedCurso, value))
                {
                    LoadCalificaciones();
                }
            }
        }

        public int TrimestreSeleccionado
        {
            get => _trimestreSeleccionado;
            set
            {
                if (SetProperty(ref _trimestreSeleccionado, value))
                {
                    LoadCalificaciones();
                }
            }
        }

        public ICommand GuardarCambiosCommand { get; }
        public ICommand ExportarExcelCommand { get; }

        private void LoadMaterias()
        {
            _materiasActuales = _context.Materias.OrderBy(m => m.Nombre).ToList();
        }

        private void LoadCursos()
        {
            var oldSelectedId = _selectedCurso?.Id;
            Cursos = new ObservableCollection<Curso>(_context.Cursos.ToList());
            
            if (oldSelectedId.HasValue)
                SelectedCurso = Cursos.FirstOrDefault(c => c.Id == oldSelectedId.Value) ?? Cursos.FirstOrDefault();
            else if (Cursos.Any())
                SelectedCurso = Cursos.First();
        }

        private void LoadCalificaciones()
        {
            if (SelectedCurso == null) return;

            var alumnos = _context.Alumnos
                .Where(a => a.CursoId == SelectedCurso.Id)
                .OrderBy(a => a.ApellidoNombre)
                .ToList();

            var calificacionesExistentes = _context.Calificaciones
                .Where(c => c.CursoId == SelectedCurso.Id && c.Trimestre == TrimestreSeleccionado)
                .ToList();

            var dtos = new List<AlumnoNotaDto>();

            foreach (var alu in alumnos)
            {
                var dto = new AlumnoNotaDto
                {
                    AlumnoId = alu.Id,
                    ApellidoNombre = alu.ApellidoNombre,
                    DNI = alu.DNI,
                    Grado = SelectedCurso.NombreGrado,
                    Division = SelectedCurso.Division,
                    NotasPorMateria = new Dictionary<int, decimal?>(),
                    Asistencias = calificacionesExistentes.FirstOrDefault(c => c.AlumnoId == alu.Id)?.Asistencia,
                    Observaciones = calificacionesExistentes.FirstOrDefault(c => c.AlumnoId == alu.Id)?.Observaciones ?? string.Empty
                };

                // Inicializar notas para cada materia actual
                foreach (var materia in _materiasActuales)
                {
                    var cal = calificacionesExistentes.FirstOrDefault(c => c.AlumnoId == alu.Id && c.MateriaId == materia.Id);
                    dto.NotasPorMateria[materia.Id] = cal?.Nota;
                }

                dtos.Add(dto);
            }

            AlumnosNotas = new ObservableCollection<AlumnoNotaDto>(dtos);
        }

        private void GuardarCambios()
        {
            if (SelectedCurso == null) return;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var dto in AlumnosNotas)
                {
                    foreach (var materiaEntry in dto.NotasPorMateria)
                    {
                        ActualizarOCrearCalificacion(
                            dto.AlumnoId, 
                            materiaEntry.Key, 
                            materiaEntry.Value, 
                            dto.Asistencias, 
                            dto.Observaciones);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();
                System.Windows.MessageBox.Show("Calificaciones guardadas correctamente.", "Éxito");
            }
            catch (System.Exception ex)
            {
                transaction.Rollback();
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error");
            }
        }

        private void ActualizarOCrearCalificacion(int alumnoId, int materiaId, decimal? nota, int? asistencia, string obs)
        {
            var cal = _context.Calificaciones.FirstOrDefault(c => 
                c.AlumnoId == alumnoId && 
                c.CursoId == SelectedCurso!.Id && 
                c.Trimestre == TrimestreSeleccionado && 
                c.MateriaId == materiaId);

            if (cal == null)
            {
                if (nota != null || asistencia != null || !string.IsNullOrWhiteSpace(obs))
                {
                    _context.Calificaciones.Add(new Calificacion
                    {
                        AlumnoId = alumnoId,
                        CursoId = SelectedCurso!.Id,
                        Trimestre = TrimestreSeleccionado,
                        MateriaId = materiaId,
                        Nota = nota,
                        Asistencia = asistencia,
                        Observaciones = obs
                    });
                }
            }
            else
            {
                cal.Nota = nota;
                cal.Asistencia = asistencia;
                cal.Observaciones = obs;
            }
        }

        private void ExportarExcel()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"Planilla_{SelectedCurso?.NombreGrado}_{TrimestreSeleccionado}.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    _excelService.ExportarConPlantilla(AlumnosNotas, MateriasActuales, sfd.FileName);
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error al exportar");
                }
            }
        }
    }
}
