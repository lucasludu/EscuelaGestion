using Microsoft.EntityFrameworkCore;

namespace EscuelaGestion.Models
{
    public class EscuelaContext : DbContext
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        public DbSet<Alumno> Alumnos { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<Calificacion> Calificaciones { get; set; } = null!;
        public DbSet<Materia> Materias { get; set; } = null!;

        public EscuelaContext()
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    Database.EnsureCreated();
                    
                    // Seed inicial si está vacío
                    if (!Materias.Any())
                    {
                        var materiasDefault = new List<string> 
                        { 
                            "Lengua", "Matemática", "Ciencias Sociales", 
                            "Ciencias Naturales", "Tecnología", 
                            "Ciudadanía y Participación", "Educación Religiosa" 
                        };
                        foreach (var m in materiasDefault)
                        {
                            Materias.Add(new Materia { Nombre = m });
                        }
                        SaveChanges();
                    }

                    if (!Cursos.Any())
                    {
                        var curso1 = new Curso { NombreGrado = "1° Grado", Division = "A", AnioLectivo = DateTime.Now.Year };
                        Cursos.Add(curso1);
                        Cursos.Add(new Curso { NombreGrado = "2° Grado", Division = "A", AnioLectivo = DateTime.Now.Year });
                        SaveChanges();
                    }
                    _initialized = true;
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "escuela.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones adicionales si son necesarias
            modelBuilder.Entity<Alumno>().HasIndex(a => a.DNI).IsUnique();
        }
    }
}
