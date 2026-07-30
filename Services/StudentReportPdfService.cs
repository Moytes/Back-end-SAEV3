using System.Data;
using Dapper;
using Models.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Services.IServices;

namespace Services;

/// <summary>
/// Genera el reporte PDF que descarga el tutor. Las actividades y evaluaciones
/// viven en las bases de otros microservicios (backend-clases, backend-reportes)
/// pero comparten la misma base física; se consultan por SQL directo, igual que
/// hace backend-reportes para leer tablas de backend-core.
/// </summary>
public class StudentReportPdfService(IDbConnection dbConnection) : IStudentReportPdfService
{
    public async Task<byte[]> GenerateAsync(
        StudentRecordDto student,
        IEnumerable<StudentDisabilityItemDto> disabilities,
        IEnumerable<StudentAttentionAreaItemDto> attentionAreas)
    {
        var actividades = await dbConnection.QueryAsync<ActividadReporteDto>("""
            SELECT
                m.titulo             AS Titulo,
                aa.estado            AS Estado,
                a.fecha_asignacion   AS FechaAsignacion,
                a.fecha_limite       AS FechaLimite
            FROM asignacion_alumnos aa
            JOIN asignaciones a ON a.id = aa.asignacion_id
            JOIN materiales m ON m.id = a.material_id
            WHERE aa.alumno_id = @StudentId
            ORDER BY a.fecha_asignacion DESC;
            """, new { StudentId = student.Id });

        var evaluaciones = await dbConnection.QueryAsync<EvaluacionResumenDto>("""
            SELECT 'Evaluación psicopedagógica' AS Tipo, fecha_elaboracion AS Fecha, estado AS Estado
            FROM evaluaciones_psicopedagogicas WHERE alumno_id = @StudentId
            UNION ALL
            SELECT 'Tamizaje TEA' AS Tipo, fecha AS Fecha, nivel_alerta AS Estado
            FROM tea_screenings WHERE alumno_id = @StudentId
            ORDER BY Fecha DESC;
            """, new { StudentId = student.Id });

        var nombreCompleto = string.Join(" ", new[] { student.Name, student.FatherLastName, student.MotherLastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        var registro = student.Registrations.FirstOrDefault();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Reporte del alumno").FontSize(18).Bold();
                    col.Item().Text(nombreCompleto).FontSize(14);
                    col.Item().PaddingTop(2).LineHorizontal(1);
                });

                page.Content().PaddingTop(10).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Column(info =>
                    {
                        info.Item().Text("Datos generales").Bold().FontSize(12);
                        info.Item().Text($"Fecha de nacimiento: {student.BirthDate:dd/MM/yyyy}");
                        if (registro != null)
                        {
                            info.Item().Text($"Escuela: {registro.SchoolName}");
                            info.Item().Text($"Grupo: {registro.GroupName}");
                            info.Item().Text($"Ciclo escolar: {registro.SchoolYearName}");
                        }
                    });

                    if (attentionAreas.Any())
                    {
                        col.Item().Column(section =>
                        {
                            section.Item().Text("Áreas de apoyo").Bold().FontSize(12);
                            foreach (var area in attentionAreas)
                                section.Item().Text($"• {area.AttentionAreaName}");
                        });
                    }

                    if (disabilities.Any())
                    {
                        col.Item().Column(section =>
                        {
                            section.Item().Text("Diagnóstico").Bold().FontSize(12);
                            foreach (var d in disabilities)
                                section.Item().Text($"• {d.DisabilityName}");
                        });
                    }

                    col.Item().Column(section =>
                    {
                        section.Item().Text("Actividades asignadas").Bold().FontSize(12);

                        if (!actividades.Any())
                        {
                            section.Item().Text("Sin actividades registradas.");
                        }
                        else
                        {
                            section.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(3);
                                    c.RelativeColumn(1);
                                    c.RelativeColumn(1);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Actividad").Bold();
                                    h.Cell().Text("Estado").Bold();
                                    h.Cell().Text("Fecha límite").Bold();
                                });

                                foreach (var act in actividades)
                                {
                                    table.Cell().Text(act.Titulo);
                                    table.Cell().Text(act.Estado);
                                    table.Cell().Text(act.FechaLimite?.ToString("dd/MM/yyyy") ?? "-");
                                }
                            });
                        }
                    });

                    col.Item().Column(section =>
                    {
                        section.Item().Text("Evaluaciones").Bold().FontSize(12);

                        if (!evaluaciones.Any())
                        {
                            section.Item().Text("Sin evaluaciones registradas.");
                        }
                        else
                        {
                            foreach (var ev in evaluaciones)
                                section.Item().Text($"• {ev.Tipo} — {ev.Fecha:dd/MM/yyyy} ({ev.Estado})");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generado el ");
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        });

        return document.GeneratePdf();
    }
}
