using System.Data;
using System.Reflection;
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
    private const string Navy = "#123047";
    private const string Teal = "#0F766E";
    private const string LightTeal = "#ECFDF5";
    private const string Slate = "#475569";
    private const string LightSlate = "#F8FAFC";
    private const string Border = "#E2E8F0";

    private static readonly byte[] LogoBytes = LoadLogo();

    private static byte[] LoadLogo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("logo.png", StringComparison.OrdinalIgnoreCase));
        if (resourceName == null) return [];

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) return [];

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerateAsync(
        StudentRecordDto student,
        IEnumerable<StudentDisabilityItemDto> disabilities,
        IEnumerable<StudentAttentionAreaItemDto> attentionAreas)
    {
        var actividades = (await dbConnection.QueryAsync<ActividadReporteDto>("""
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
            """, new { StudentId = student.Id })).ToList();

        var observaciones = (await dbConnection.QueryAsync<DocenteObservacionDto>("""
            SELECT
                o.id              AS Id,
                o.student_id      AS StudentId,
                o.docente_id      AS DocenteId,
                TRIM(CONCAT(u.name, ' ', u.father_last_name,
                    COALESCE(' ' || NULLIF(u.mother_last_name, ''), ''))) AS DocenteNombre,
                o.school_year_id  AS SchoolYearId,
                o.texto           AS Texto,
                o.created_at      AS CreatedAt
            FROM docente_observaciones o
            JOIN "user" u ON u.id = o.docente_id
            WHERE o.student_id = @StudentId
            ORDER BY o.created_at DESC;
            """, new { StudentId = student.Id })).ToList();

        var evaluaciones = (await dbConnection.QueryAsync<EvaluacionResumenDto>("""
            SELECT 'Evaluación psicopedagógica' AS Tipo, fecha_elaboracion AS Fecha, estado AS Estado
            FROM evaluaciones_psicopedagogicas WHERE alumno_id = @StudentId
            UNION ALL
            SELECT 'Tamizaje TEA' AS Tipo, fecha AS Fecha, nivel_alerta AS Estado
            FROM tea_screenings WHERE alumno_id = @StudentId
            ORDER BY Fecha DESC;
            """, new { StudentId = student.Id })).ToList();

        var disabilityList = disabilities.ToList();
        var attentionAreaList = attentionAreas.ToList();

        var nombreCompleto = string.Join(" ", new[] { student.Name, student.FatherLastName, student.MotherLastName }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        var registro = student.Registrations.FirstOrDefault();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(1.65f, Unit.Centimetre);
                page.MarginVertical(1.25f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(Navy));

                page.Header().Column(header =>
                {
                    header.Item().Height(4).Background(Teal);
                    header.Item().Element(c => ComposeHeader(c, nombreCompleto));
                });

                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Spacing(16);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => SummaryCard(
                            c, actividades.Count.ToString(), "Actividades", Teal));
                        row.Spacing(8);
                        row.RelativeItem().Element(c => SummaryCard(
                            c, actividades.Count(a => a.Estado is "COMPLETADO" or "EVALUADO").ToString(),
                            "Completadas", "#2563EB"));
                        row.Spacing(8);
                        row.RelativeItem().Element(c => SummaryCard(
                            c, evaluaciones.Count.ToString(), "Evaluaciones", "#7C3AED"));
                        row.Spacing(8);
                        row.RelativeItem().Element(c => SummaryCard(
                            c, observaciones.Count.ToString(), "Observaciones", "#D97706"));
                    });

                    col.Item().Element(c => SectionCard(c, "Datos generales", content =>
                    {
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            DetailCell(table, "Fecha de nacimiento", student.BirthDate.ToString("dd/MM/yyyy"));
                            DetailCell(table, "CURP", student.CURP ?? "No registrada");
                            DetailCell(table, "Escuela", registro?.SchoolName ?? "No registrada");
                            DetailCell(table, "Grupo", registro?.GroupName ?? "No registrado");
                            DetailCell(table, "Ciclo escolar", registro?.SchoolYearName ?? "No registrado");
                            DetailCell(table, "Fecha del reporte", DateTime.Now.ToString("dd/MM/yyyy"));
                        });
                    }));

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => InfoListCard(
                            c,
                            "Áreas de apoyo",
                            attentionAreaList.Select(a => a.AttentionAreaName).ToList(),
                            "Sin áreas de apoyo registradas."));
                        row.Spacing(10);
                        row.RelativeItem().Element(c => InfoListCard(
                            c,
                            "Necesidades identificadas",
                            disabilityList.Select(d => d.DisabilityName).ToList(),
                            "Sin necesidades registradas."));
                    });

                    col.Item().Element(c => SectionCard(c, "Plan de acción · Actividades asignadas", content =>
                    {
                        if (actividades.Count == 0)
                        {
                            content.Item().Element(c => EmptyState(c, "Sin actividades registradas."));
                        }
                        else
                        {
                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(3.2f);
                                    c.RelativeColumn(1.2f);
                                    c.RelativeColumn(1.3f);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(TableHeaderCell).Text("Actividad");
                                    h.Cell().Element(TableHeaderCell).Text("Estado");
                                    h.Cell().Element(TableHeaderCell).Text("Fecha límite");
                                });

                                for (var i = 0; i < actividades.Count; i++)
                                {
                                    var act = actividades[i];
                                    var background = i % 2 == 0 ? "#FFFFFF" : LightSlate;
                                    table.Cell().Element(c => TableBodyCell(c, background)).Text(act.Titulo);
                                    table.Cell().Element(c => TableBodyCell(c, background))
                                        .Text(StatusLabel(act.Estado))
                                        .FontColor(StatusColor(act.Estado))
                                        .SemiBold();
                                    table.Cell().Element(c => TableBodyCell(c, background))
                                        .Text(act.FechaLimite?.ToString("dd/MM/yyyy") ?? "Sin fecha");
                                }
                            });
                        }
                    }));

                    col.Item().Element(c => SectionCard(c, "Observaciones del docente", content =>
                    {
                        if (observaciones.Count == 0)
                        {
                            content.Item().Element(c => EmptyState(c, "Sin observaciones registradas."));
                        }
                        else
                        {
                            foreach (var obs in observaciones.Take(12))
                            {
                                var autor = string.IsNullOrWhiteSpace(obs.DocenteNombre) ? "Docente" : obs.DocenteNombre;
                                content.Item()
                                    .PaddingBottom(8)
                                    .BorderBottom(1)
                                    .BorderColor(Border)
                                    .Column(item =>
                                    {
                                        item.Item().Text($"{autor} · {obs.CreatedAt:dd/MM/yyyy HH:mm}")
                                            .FontSize(8).SemiBold().FontColor(Teal);
                                        item.Item().PaddingTop(3).Text(obs.Texto)
                                            .FontSize(9).FontColor(Slate).LineHeight(1.35f);
                                    });
                            }
                        }
                    }));

                    col.Item().Element(c => SectionCard(c, "Evaluaciones registradas", content =>
                    {
                        if (evaluaciones.Count == 0)
                        {
                            content.Item().Element(c => EmptyState(c, "Sin evaluaciones registradas."));
                        }
                        else
                        {
                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2.5f);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });
                                table.Header(header =>
                                {
                                    header.Cell().Element(TableHeaderCell).Text("Tipo");
                                    header.Cell().Element(TableHeaderCell).Text("Fecha");
                                    header.Cell().Element(TableHeaderCell).Text("Resultado / estado");
                                });
                                for (var i = 0; i < evaluaciones.Count; i++)
                                {
                                    var ev = evaluaciones[i];
                                    var background = i % 2 == 0 ? "#FFFFFF" : LightSlate;
                                    table.Cell().Element(c => TableBodyCell(c, background)).Text(ev.Tipo);
                                    table.Cell().Element(c => TableBodyCell(c, background)).Text($"{ev.Fecha:dd/MM/yyyy}");
                                    table.Cell().Element(c => TableBodyCell(c, background)).Text(StatusLabel(ev.Estado));
                                }
                            });
                        }
                    }));

                    col.Item().PaddingTop(4).Background("#FFF7ED").Border(1).BorderColor("#FED7AA")
                        .Padding(10).Text(
                            "Documento informativo de seguimiento educativo. La información contenida es confidencial y debe utilizarse exclusivamente para fines de atención y acompañamiento del alumno.")
                        .FontSize(7.5f).FontColor("#9A3412").LineHeight(1.3f);
                });

                page.Footer().PaddingTop(8).BorderTop(1).BorderColor(Border).Row(row =>
                {
                    if (LogoBytes.Length > 0)
                    {
                        row.ConstantItem(14).Height(14).Image(LogoBytes).FitArea();
                        row.ConstantItem(6);
                    }
                    row.RelativeItem().AlignMiddle().Text("SiembraEdu · Sistema Integral de Atención Educativa")
                        .FontSize(7).FontColor(Slate);
                    row.RelativeItem().AlignRight().AlignMiddle().Text(x =>
                    {
                        x.DefaultTextStyle(s => s.FontSize(7).FontColor(Slate));
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, string studentName)
    {
        container.Background(Navy).Padding(16).Row(row =>
        {
            if (LogoBytes.Length > 0)
            {
                row.ConstantItem(52).AlignMiddle().Background(Colors.White)
                    .CornerRadius(10).Padding(4).Height(44).Width(44)
                    .Image(LogoBytes).FitArea();
                row.ConstantItem(12);
            }

            row.RelativeItem().AlignMiddle().Column(column =>
            {
                column.Item().Text("SIEMBRAEDU").FontSize(8).Bold().FontColor("#5EEAD4").LetterSpacing(0.12f);
                column.Item().PaddingTop(3).Text("Reporte integral del alumno")
                    .FontSize(18).Bold().FontColor(Colors.White);
                column.Item().PaddingTop(2).Text(studentName)
                    .FontSize(11).FontColor("#CBD5E1");
            });
            row.ConstantItem(115).AlignRight().AlignMiddle().Column(column =>
            {
                column.Item().AlignRight().Text("REPORTE EDUCATIVO")
                    .FontSize(7).Bold().FontColor("#99F6E4");
                column.Item().PaddingTop(4).AlignRight().Text(DateTime.Now.ToString("dd MMM yyyy"))
                    .FontSize(8).FontColor(Colors.White);
            });
        });
    }

    private static void SummaryCard(IContainer container, string value, string label, string accent)
    {
        container.Border(1).BorderColor(Border).Background(Colors.White).Column(column =>
        {
            column.Item().Height(3).Background(accent);
            column.Item().Padding(10).Column(inner =>
            {
                inner.Item().Text(value).FontSize(18).Bold().FontColor(accent);
                inner.Item().Text(label).FontSize(7.5f).FontColor(Slate);
            });
        });
    }

    private static void SectionCard(IContainer container, string title, Action<ColumnDescriptor> content)
    {
        container.Border(1).BorderColor(Border).Background(Colors.White).Column(column =>
        {
            column.Item().Background(LightTeal).BorderBottom(1).BorderColor("#A7F3D0")
                .PaddingVertical(8).PaddingHorizontal(12)
                .Text(title).FontSize(10).Bold().FontColor(Teal);
            column.Item().Padding(12).Column(content);
        });
    }

    private static void InfoListCard(IContainer container, string title, IReadOnlyList<string> values, string emptyText)
    {
        SectionCard(container, title, column =>
        {
            if (values.Count == 0)
            {
                column.Item().Text(emptyText).FontSize(8.5f).Italic().FontColor(Slate);
                return;
            }

            foreach (var value in values)
                column.Item().PaddingBottom(3).Text($"•  {value}").FontSize(8.5f).FontColor(Slate);
        });
    }

    private static void DetailCell(TableDescriptor table, string label, string value)
    {
        table.Cell().PaddingBottom(8).Column(column =>
        {
            column.Item().Text(label.ToUpperInvariant()).FontSize(6.5f).Bold().FontColor("#64748B");
            column.Item().PaddingTop(2).Text(value).FontSize(9).FontColor(Navy);
        });
    }

    private static IContainer TableHeaderCell(IContainer container) =>
        container.Background(Navy).PaddingVertical(7).PaddingHorizontal(8)
            .DefaultTextStyle(x => x.FontSize(7.5f).Bold().FontColor(Colors.White));

    private static IContainer TableBodyCell(IContainer container, string background) =>
        container.Background(background).BorderBottom(1).BorderColor(Border)
            .PaddingVertical(7).PaddingHorizontal(8)
            .DefaultTextStyle(x => x.FontSize(8).FontColor(Navy));

    private static void EmptyState(IContainer container, string message) =>
        container.Background(LightSlate).Padding(10).AlignCenter()
            .Text(message).FontSize(8.5f).Italic().FontColor(Slate);

    private static string StatusLabel(string? status) => status?.ToUpperInvariant() switch
    {
        "PENDIENTE" => "Pendiente",
        "EN_PROGRESO" or "EN_PROCESO" => "En proceso",
        "COMPLETADO" or "COMPLETADA" => "Completado",
        "EVALUADO" => "Evaluado",
        "BORRADOR" => "Borrador",
        "EN_REVISION" => "En revisión",
        "FIRMADA" => "Firmada",
        "ENTREGADA" => "Entregada",
        "SIN_ALERTA" => "Sin alerta",
        "LEVE" => "Leve",
        "MODERADO" => "Moderado",
        "SIGNIFICATIVO" => "Significativo",
        _ => status?.Replace('_', ' ') ?? "Sin estado"
    };

    private static string StatusColor(string? status) => status?.ToUpperInvariant() switch
    {
        "COMPLETADO" or "EVALUADO" => "#15803D",
        "EN_PROGRESO" => "#1D4ED8",
        "PENDIENTE" => "#B45309",
        _ => Slate
    };
}
