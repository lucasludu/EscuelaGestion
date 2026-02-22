using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using EscuelaGestion.Models;

namespace EscuelaGestion.Services
{
    public interface IExcelService
    {
        void ExportarConPlantilla(IEnumerable<AlumnoNotaDto> data, IEnumerable<Materia> materias, string targetPath);
    }

    public class ExcelService : IExcelService
    {
        public void ExportarConPlantilla(IEnumerable<AlumnoNotaDto> data, IEnumerable<Materia> materias, string targetPath)
        {
            // Nota: Podrías seguir usando una plantilla, pero para que sea dinámico 
            // es mejor generar el encabezado según las materias actuales.
            
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Notas");
                
                // --- ENCABEZADOS ---
                ws.Cell(1, 1).Value = "PLANILLA DE CALIFICACIONES";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;

                int col = 1;
                ws.Cell(3, col++).Value = "N° Orden";
                ws.Cell(3, col++).Value = "Apellido y Nombre";
                ws.Cell(3, col++).Value = "DNI";
                ws.Cell(3, col++).Value = "Grado";
                ws.Cell(3, col++).Value = "División";

                // Columnas dinámicas para materias
                int colInicioMaterias = col;
                foreach (var materia in materias)
                {
                    ws.Cell(3, col++).Value = materia.Nombre;
                }

                ws.Cell(3, col++).Value = "Inasistencias";
                ws.Cell(3, col++).Value = "Observaciones";

                // Estilo para encabezados
                var headerRange = ws.Range(3, 1, 3, col - 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // --- DATOS ---
                int row = 4;
                int orden = 1;

                foreach (var item in data)
                {
                    int currentCol = 1;
                    ws.Cell(row, currentCol++).Value = orden++;
                    ws.Cell(row, currentCol++).Value = item.ApellidoNombre;
                    ws.Cell(row, currentCol++).Value = item.DNI;
                    ws.Cell(row, currentCol++).Value = item.Grado;
                    ws.Cell(row, currentCol++).Value = item.Division;

                    // Notas dinámicas
                    foreach (var materia in materias)
                    {
                        if (item.NotasPorMateria.TryGetValue(materia.Id, out decimal? nota))
                        {
                            ws.Cell(row, currentCol).Value = nota;
                        }
                        currentCol++;
                    }

                    ws.Cell(row, currentCol++).Value = item.Asistencias;
                    ws.Cell(row, currentCol++).Value = item.Observaciones;

                    row++;
                }

                // Autoajustar columnas
                ws.Columns().AdjustToContents();
                
                workbook.SaveAs(targetPath);
            }
        }
    }
}
