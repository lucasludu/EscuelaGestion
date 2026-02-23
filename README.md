# 🏫 Libreta Digital - Gestión Escolar

**Libreta Digital** es una aplicación de escritorio desarrollada en **C#** utilizando **WPF** y **.NET 10**. Está diseñada para facilitar la gestión académica de una institución educativa, permitiendo administrar alumnos, cursos, materias y calificaciones de manera eficiente y centralizada.

## 🚀 Características Principales

-   **👤 Gestión de Alumnos:** Registro, edición y eliminación de estudiantes con validación de DNI y asignación a cursos/años específicos.
-   **📚 Administración de Cursos y Materias:** Módulo de configuración para crear y organizar los grados, divisiones y materias que dicta la institución.
-   **📝 Calificaciones y Asistencia:** 
    -   Carga de notas por trimestre (1°, 2°, 3° y Final).
    -   Seguimiento de inasistencias por alumno.
    -   Espacio para observaciones pedagógicas.
-   **📊 Exportación de Datos:** Generación de planillas de calificaciones directamente a formato **Excel** para facilitar reportes y archivo físico.
-   **💾 Almacenamiento Local:** Base de datos ligera mediante **SQLite** y **Entity Framework Core**, lo que elimina la necesidad de configuraciones de servidor complejas.

## 🛠️ Tecnologías Utilizadas

-   **Lenguaje:** [C#](https://learn.microsoft.com/es-es/dotnet/csharp/)
-   **Framework UI:** [WPF (Windows Presentation Foundation)](https://learn.microsoft.com/es-es/dotnet/desktop/wpf/)
-   **Plataforma:** .NET 10
-   **Base de Datos:** SQLite
-   **ORM:** Entity Framework Core
-   **Librerías de Terceros:**
    -   [ClosedXML](https://github.com/ClosedXML/ClosedXML) (Para la generación de archivos Excel).

## 📋 Requisitos

-   Windows 10 o superior.
-   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.

## ⚙️ Instalación y Ejecución

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/tu-usuario/EscuelaGestion.git](https://github.com/tu-usuario/EscuelaGestion.git)
    cd EscuelaGestion
    ```

2.  **Restaurar dependencias:**
    ```bash
    dotnet restore
    ```

3.  **Ejecutar la aplicación:**
    ```bash
    dotnet run
    ```

*(Nota: Al iniciar por primera vez, EF Core se encargará de crear la base de datos local `LibretaDigital.db` si no existe).*

## 📖 Uso de la Aplicación

1.  **Configuración Inicial:** Ve a la pestaña de **Configuración** para agregar las materias y los cursos del año lectivo actual.
2.  **Carga de Alumnos:** En la pestaña **Alumnos**, ingresa los datos de los estudiantes y asígnalos a sus respectivos cursos.
3.  **Calificar:** Selecciona un curso y un trimestre en la pestaña **Calificaciones**. Las materias aparecerán automáticamente como columnas. Completa las notas y presiona **Guardar**.
4.  **Reportes:** Usa el botón **Generar Planilla Excel** para obtener un documento con todas las notas del curso seleccionado.

---
⭐ *Desarrollado para simplificar la labor docente.*
