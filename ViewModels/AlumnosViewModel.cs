using EscuelaGestion.Helpers;
using EscuelaGestion.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EscuelaGestion.ViewModels
{
    public class AlumnosViewModel : ViewModelBase
    {
        private readonly EscuelaContext _context;
        private ObservableCollection<Alumno> _alumnos = null!;
        private ObservableCollection<Curso> _cursos = null!;
        private Alumno? _selectedAlumno;
        private Curso? _selectedCursoForNewAlumno;
        private string _nuevoNombre = string.Empty;
        private string _nuevoDNI = string.Empty;

        public AlumnosViewModel()
        {
            _context = new EscuelaContext();
            LoadData();
            
            AgregarCommand = new RelayCommand(_ => AgregarAlumno(), _ => CanAgregar());
            EliminarCommand = new RelayCommand(_ => EliminarAlumno(), _ => SelectedAlumno != null);
            GuardarCambiosCommand = new RelayCommand(_ => GuardarCambios());

            MessageHub.ConfigurationChanged += () => LoadData();
        }

        public ObservableCollection<Alumno> Alumnos
        {
            get => _alumnos;
            set => SetProperty(ref _alumnos, value);
        }

        public ObservableCollection<Curso> Cursos
        {
            get => _cursos;
            set => SetProperty(ref _cursos, value);
        }

        public Alumno? SelectedAlumno
        {
            get => _selectedAlumno;
            set => SetProperty(ref _selectedAlumno, value);
        }

        public Curso? SelectedCursoForNewAlumno
        {
            get => _selectedCursoForNewAlumno;
            set => SetProperty(ref _selectedCursoForNewAlumno, value);
        }

        public string NuevoNombre
        {
            get => _nuevoNombre;
            set => SetProperty(ref _nuevoNombre, value);
        }

        public string NuevoDNI
        {
            get => _nuevoDNI;
            set => SetProperty(ref _nuevoDNI, value);
        }

        public ICommand AgregarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand GuardarCambiosCommand { get; }

        private void LoadData()
        {
            var oldSelectedCursoId = _selectedCursoForNewAlumno?.Id;

            // Cargamos directamente desde la base de datos para ignorar el cache local si hubo cambios externos
            var alumnosList = _context.Alumnos.Include(a => a.Curso).OrderBy(a => a.ApellidoNombre).ToList();
            Alumnos = new ObservableCollection<Alumno>(alumnosList);
            
            var cursosList = _context.Cursos.ToList();
            Cursos = new ObservableCollection<Curso>(cursosList);
            
            if (oldSelectedCursoId.HasValue)
                SelectedCursoForNewAlumno = Cursos.FirstOrDefault(c => c.Id == oldSelectedCursoId.Value) ?? Cursos.FirstOrDefault();
            else if (Cursos.Any())
                SelectedCursoForNewAlumno = Cursos.First();
        }

        private bool CanAgregar() => !string.IsNullOrWhiteSpace(NuevoNombre) && !string.IsNullOrWhiteSpace(NuevoDNI) && SelectedCursoForNewAlumno != null;

        private void AgregarAlumno()
        {
            var alumno = new Alumno 
            { 
                ApellidoNombre = NuevoNombre, 
                DNI = NuevoDNI, 
                CursoId = SelectedCursoForNewAlumno?.Id 
            };
            _context.Alumnos.Add(alumno);
            _context.SaveChanges();
            
            NuevoNombre = string.Empty;
            NuevoDNI = string.Empty;

            // Recargar la lista para que se muestre el nuevo alumno con su curso
            LoadData();
        }

        private void EliminarAlumno()
        {
            if (SelectedAlumno != null)
            {
                _context.Alumnos.Remove(SelectedAlumno);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void GuardarCambios()
        {
            _context.SaveChanges();
            System.Windows.MessageBox.Show("Cambios guardados correctamente.");
        }
    }
}
