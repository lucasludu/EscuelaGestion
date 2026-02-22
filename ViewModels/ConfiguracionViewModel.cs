using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EscuelaGestion.Helpers;
using EscuelaGestion.Models;
using Microsoft.EntityFrameworkCore;

namespace EscuelaGestion.ViewModels
{
    public class ConfiguracionViewModel : ViewModelBase
    {
        private readonly EscuelaContext _context;
        private ObservableCollection<Materia> _materias = new();
        private ObservableCollection<Curso> _cursos = new();
        private Materia? _selectedMateria;
        private Curso? _selectedCurso;
        private string _nuevaMateriaNombre = string.Empty;
        private string _nuevoGradoNombre = string.Empty;
        private string _nuevaDivision = string.Empty;
        private int _anioLectivoActual;

        public ConfiguracionViewModel()
        {
            _context = new EscuelaContext();
            LoadMaterias();
            LoadCursos();
            LoadConfiguracion();

            AgregarMateriaCommand = new RelayCommand(_ => AgregarMateria(), _ => !string.IsNullOrWhiteSpace(NuevaMateriaNombre));
            EliminarMateriaCommand = new RelayCommand(_ => EliminarMateria(), _ => SelectedMateria != null);
            
            AgregarCursoCommand = new RelayCommand(_ => AgregarCurso(), _ => !string.IsNullOrWhiteSpace(NuevoGradoNombre) && !string.IsNullOrWhiteSpace(NuevaDivision));
            EliminarCursoCommand = new RelayCommand(_ => EliminarCurso(), _ => SelectedCurso != null);
            
            GuardarConfiguracionCommand = new RelayCommand(_ => GuardarConfiguracion());
            GuardarCambiosCommand = new RelayCommand(_ => GuardarCambios());
        }

        public ObservableCollection<Materia> Materias
        {
            get => _materias;
            set => SetProperty(ref _materias, value);
        }

        public ObservableCollection<Curso> Cursos
        {
            get => _cursos;
            set => SetProperty(ref _cursos, value);
        }

        public Materia? SelectedMateria
        {
            get => _selectedMateria;
            set => SetProperty(ref _selectedMateria, value);
        }

        public Curso? SelectedCurso
        {
            get => _selectedCurso;
            set => SetProperty(ref _selectedCurso, value);
        }

        public string NuevaMateriaNombre
        {
            get => _nuevaMateriaNombre;
            set => SetProperty(ref _nuevaMateriaNombre, value);
        }

        public string NuevoGradoNombre
        {
            get => _nuevoGradoNombre;
            set => SetProperty(ref _nuevoGradoNombre, value);
        }

        public string NuevaDivision
        {
            get => _nuevaDivision;
            set => SetProperty(ref _nuevaDivision, value);
        }

        public int AnioLectivoActual
        {
            get => _anioLectivoActual;
            set => SetProperty(ref _anioLectivoActual, value);
        }

        public ICommand AgregarMateriaCommand { get; }
        public ICommand EliminarMateriaCommand { get; }
        public ICommand AgregarCursoCommand { get; }
        public ICommand EliminarCursoCommand { get; }
        public ICommand GuardarConfiguracionCommand { get; }
        public ICommand GuardarCambiosCommand { get; }

        private void LoadMaterias()
        {
            _context.Materias.Load();
            Materias = _context.Materias.Local.ToObservableCollection();
        }

        private void LoadCursos()
        {
            _context.Cursos.Load();
            Cursos = _context.Cursos.Local.ToObservableCollection();
        }

        private void LoadConfiguracion()
        {
            AnioLectivoActual = _context.Cursos.OrderByDescending(c => c.AnioLectivo).FirstOrDefault()?.AnioLectivo ?? 2024;
        }

        private void AgregarMateria()
        {
            if (_context.Materias.Any(m => m.Nombre.ToLower() == NuevaMateriaNombre.ToLower()))
            {
                System.Windows.MessageBox.Show("La materia ya existe.", "Aviso");
                return;
            }

            var nueva = new Materia { Nombre = NuevaMateriaNombre };
            _context.Materias.Add(nueva);
            _context.SaveChanges();
            
            NuevaMateriaNombre = string.Empty;
            LoadMaterias();
            MessageHub.NotifyConfigurationChanged();
        }

        private void EliminarMateria()
        {
            if (SelectedMateria == null) return;

            if (_context.Calificaciones.Any(c => c.MateriaId == SelectedMateria.Id))
            {
                System.Windows.MessageBox.Show("No se puede eliminar la materia porque tiene calificaciones asociadas.", "Error");
                return;
            }

            if (System.Windows.MessageBox.Show($"¿Está seguro de eliminar '{SelectedMateria.Nombre}'?", "Confirmar", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Materias.Remove(SelectedMateria);
                _context.SaveChanges();
                MessageHub.NotifyConfigurationChanged();
            }
        }

        private void AgregarCurso()
        {
            if (_context.Cursos.Any(c => c.NombreGrado.ToLower() == NuevoGradoNombre.ToLower() && c.Division.ToLower() == NuevaDivision.ToLower() && c.AnioLectivo == AnioLectivoActual))
            {
                System.Windows.MessageBox.Show("El curso ya existe para este año lectivo.", "Aviso");
                return;
            }

            var nuevo = new Curso 
            { 
                NombreGrado = NuevoGradoNombre, 
                Division = NuevaDivision, 
                AnioLectivo = AnioLectivoActual 
            };
            _context.Cursos.Add(nuevo);
            _context.SaveChanges();
            
            NuevoGradoNombre = string.Empty;
            NuevaDivision = string.Empty;
            MessageHub.NotifyConfigurationChanged();
        }

        private void EliminarCurso()
        {
            if (SelectedCurso == null) return;

            if (_context.Alumnos.Any(a => a.CursoId == SelectedCurso.Id))
            {
                System.Windows.MessageBox.Show("No se puede eliminar el curso porque tiene alumnos asociados.", "Error");
                return;
            }

            if (System.Windows.MessageBox.Show($"¿Está seguro de eliminar '{SelectedCurso.NombreGrado} {SelectedCurso.Division}'?", "Confirmar", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                _context.Cursos.Remove(SelectedCurso);
                _context.SaveChanges();
                MessageHub.NotifyConfigurationChanged();
            }
        }

        private void GuardarCambios()
        {
            _context.SaveChanges();
            MessageHub.NotifyConfigurationChanged();
            System.Windows.MessageBox.Show("Todos los cambios en materias y cursos han sido guardados.", "Éxito");
        }

        private void GuardarConfiguracion()
        {
            _context.SaveChanges();
            MessageHub.NotifyConfigurationChanged();
            System.Windows.MessageBox.Show($"Año lectivo guardado: {AnioLectivoActual}.", "Configuración Guardada");
        }
    }
}
