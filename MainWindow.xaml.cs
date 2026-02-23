using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using EscuelaGestion.ViewModels;
using System.Collections.Generic;

namespace EscuelaGestion
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Helpers.MessageHub.ConfigurationChanged += MessageHub_ConfigurationChanged;
        }

        private void MessageHub_ConfigurationChanged()
        {
            // Forzar actualización si estamos en la pestaña de calificaciones
            if (tabControl.SelectedItem is TabItem tabItem && tabItem.Header?.ToString() == "Calificaciones")
            {
                if (tabItem.DataContext is CalificacionesViewModel vm)
                {
                    vm.RefreshAll();
                    GenerarColumnasCalificaciones(vm);
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl && tabControl.SelectedItem is TabItem tabItem)
            {
                // Si cambiamos a la pestaña de calificaciones
                if (tabItem.Header?.ToString() == "Calificaciones")
                {
                    if (tabItem.DataContext is CalificacionesViewModel vm)
                    {
                        vm.RefreshAll();
                        GenerarColumnasCalificaciones(vm);
                    }
                }
                // Si volvemos de configuración a alumnos, también refrescamos
                else if (tabItem.Header?.ToString() == "Alumnos")
                {
                    if (tabItem.DataContext is AlumnosViewModel vm)
                    {
                        // Podrías añadir un Refresh si fuera necesario
                    }
                }
            }
        }

        private void GenerarColumnasCalificaciones(CalificacionesViewModel vm)
        {
            // Limpiar columnas dinámicas previas (mantener Alumno, Inasistencias y Observaciones)
            // Las columnas básicas son la 0 (Alumno) y las dos últimas (Inasistencias y Observaciones)
            
            // Reconstruimos la colección de columnas para evitar líos
            dgCalificaciones.Columns.Clear();

            // 1. Columna fija Alumno
            dgCalificaciones.Columns.Add(new DataGridTextColumn 
            { 
                Header = "Alumno", 
                Binding = new Binding("ApellidoNombre"), 
                IsReadOnly = true, 
                Width = 150 
            });

            dgCalificaciones.Columns.Add(new DataGridTextColumn 
            { 
                Header = "Grado", 
                Binding = new Binding("Grado"), 
                IsReadOnly = true, 
                Width = 100 
            });

            dgCalificaciones.Columns.Add(new DataGridTextColumn 
            { 
                Header = "División", 
                Binding = new Binding("Division"), 
                IsReadOnly = true, 
                Width = 60 
            });

            // 2. Generar columnas para cada materia
            foreach (var materia in vm.MateriasActuales)
            {
                var column = new DataGridComboBoxColumn
                {
                    Header = materia.Nombre,
                    // Binding dinámico al diccionario: NotasPorMateria[Id]
                    SelectedValueBinding = new Binding($"NotasPorMateria[{materia.Id}]") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                    // Usar la escala definida en el ViewModel
                    ItemsSource = vm.EscalaNotas,
                    Width = 80
                };
                dgCalificaciones.Columns.Add(column);
            }

            // 3. Columnas fijas finales
            dgCalificaciones.Columns.Add(new DataGridTextColumn 
            { 
                Header = "Inasistencias", 
                Binding = new Binding("Asistencias"), 
                Width = 80 
            });
            dgCalificaciones.Columns.Add(new DataGridTextColumn 
            { 
                Header = "Observaciones", 
                Binding = new Binding("Observaciones"), 
                Width = new DataGridLength(1, DataGridLengthUnitType.Star) 
            });
        }
    }
}
