using System.Security.Claims;
using System.Data;
using Dapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Repositories.IRepositories;
using Services.IServices;

namespace Controllers;

public record CreateDocenteObservacionRequest(string Texto, int? SchoolYearId = null);

[ApiController]
[Route("api/docente")]
[Produces("application/json")]
[Authorize(Roles = "DOCENTE")]
public class DocenteController(
    AppDbContext context,
    IStudentRepositorie studentRepositorie,
    IStudentSupportRepositorie studentSupportRepositorie,
    IStudentReportPdfService studentReportPdfService,
    IDbConnection dbConnection) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
    private readonly IStudentSupportRepositorie _studentSupportRepositorie = studentSupportRepositorie;
    private readonly IStudentReportPdfService _studentReportPdfService = studentReportPdfService;
    private readonly IDbConnection _dbConnection = dbConnection;

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools([FromQuery] int? schoolYearId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var schools = await _context.UserSchool
            .AsNoTracking()
            .Include(us => us.School)
                .ThenInclude(s => s.EducationLevel)
            .Include(us => us.School)
                .ThenInclude(s => s.SchoolZone)
            .Where(us => us.UserId == userId.Value && (!schoolYearId.HasValue || us.SchoolYearId == schoolYearId.Value))
            .OrderBy(us => us.School.Name)
            .Select(us => new
            {
                us.School.Id,
                us.School.Name,
                us.School.CCT,
                us.School.EducationLevelId,
                EducationLevelName = us.School.EducationLevel.Nombre,
                us.School.SchoolZoneId,
                SchoolZoneName = us.School.SchoolZone != null ? us.School.SchoolZone.Name : null
            })
            .Distinct()
            .ToListAsync();

        return Ok(schools);
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> GetGroups([FromQuery] int? schoolId = null, [FromQuery] int? schoolYearId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var groups = await _context.UserGroup
            .AsNoTracking()
            .Include(ug => ug.Group)
                .ThenInclude(g => g.School)
            .Include(ug => ug.Group)
                .ThenInclude(g => g.Grade)
                    .ThenInclude(g => g.EducationLevel)
            .Include(ug => ug.SchoolYear)
            .Where(ug =>
                ug.UserId == userId.Value &&
                (!schoolId.HasValue || ug.Group.SchoolId == schoolId.Value) &&
                (!schoolYearId.HasValue || ug.SchoolYearId == schoolYearId.Value))
            .OrderByDescending(ug => ug.SchoolYear.Name)
            .ThenBy(ug => ug.Group.School.Name)
            .ThenBy(ug => ug.Group.Grade.Numero)
            .ThenBy(ug => ug.Group.Section)
            .Select(ug => new
            {
                ug.Group.Id,
                ug.Group.SchoolId,
                SchoolName = ug.Group.School.Name,
                SchoolCCT = ug.Group.School.CCT,
                SchoolYearId = ug.SchoolYearId,
                SchoolYearName = ug.SchoolYear.Name,
                ug.Group.GradeId,
                GradeName = ug.Group.Grade.Nombre,
                GradeNumber = ug.Group.Grade.Numero,
                EducationLevelId = ug.Group.Grade.EducationLevelId,
                EducationLevelName = ug.Group.Grade.EducationLevel.Nombre,
                ug.Group.Section,
                DisplayName = $"{ug.Group.Grade.Numero}° {ug.Group.Section}",
                ug.EsTitular
            })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] int? groupId = null,
        [FromQuery] int? schoolYearId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var allowedGroupIds = await _context.UserGroup
            .AsNoTracking()
            .Where(ug =>
                ug.UserId == userId.Value &&
                (!groupId.HasValue || ug.GroupId == groupId.Value) &&
                (!schoolYearId.HasValue || ug.SchoolYearId == schoolYearId.Value))
            .Select(ug => new { ug.GroupId, ug.SchoolYearId })
            .ToListAsync();

        if (allowedGroupIds.Count == 0)
            return Ok(Array.Empty<object>());

        var groupIds = allowedGroupIds.Select(g => g.GroupId).Distinct().ToArray();
        var yearIds = allowedGroupIds.Select(g => g.SchoolYearId).Distinct().ToArray();
        var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        var students = await _context.Registration
            .AsNoTracking()
            .Include(r => r.Student)
            .Include(r => r.Group)
                .ThenInclude(g => g.School)
            .Include(r => r.Group)
                .ThenInclude(g => g.Grade)
            .Include(r => r.SchoolYear)
            .Where(r =>
                groupIds.Contains(r.GroupId) &&
                yearIds.Contains(r.SchoolYearId) &&
                (normalizedSearch == null ||
                    EF.Functions.ILike(r.Student.Name, $"%{normalizedSearch}%") ||
                    EF.Functions.ILike(r.Student.FatherLastName, $"%{normalizedSearch}%") ||
                    (r.Student.MotherLastName != null && EF.Functions.ILike(r.Student.MotherLastName, $"%{normalizedSearch}%")) ||
                    (r.Student.CURP != null && EF.Functions.ILike(r.Student.CURP, $"%{normalizedSearch}%"))))
            .OrderBy(r => r.Student.FatherLastName)
            .ThenBy(r => r.Student.MotherLastName)
            .ThenBy(r => r.Student.Name)
            .Select(r => new
            {
                r.Student.Id,
                r.Student.Name,
                r.Student.FatherLastName,
                r.Student.MotherLastName,
                Gender = r.Student.Sexo,
                r.Student.BirthDate,
                Curp = r.Student.CURP,
                r.Student.PhotoUrl,
                Status = r.Student.Activo ? 1 : 0,
                SchoolId = r.Group.SchoolId,
                SchoolName = r.Group.School.Name,
                GroupId = r.GroupId,
                GroupName = $"{r.Group.Grade.Numero}° {r.Group.Section}",
                GradeId = r.Group.GradeId,
                SchoolYearId = r.SchoolYearId,
                SchoolYearName = r.SchoolYear.Name
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("alumnos/{id:guid}/observaciones")]
    public async Task<IActionResult> GetObservaciones(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!(await GetAllowedStudentIdsAsync(userId.Value)).Contains(id))
            return Forbid();

        var observaciones = await QueryObservacionesAsync(id);
        return Ok(observaciones);
    }

    [HttpPost("alumnos/{id:guid}/observaciones")]
    public async Task<IActionResult> CreateObservacion(Guid id, [FromBody] CreateDocenteObservacionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!(await GetAllowedStudentIdsAsync(userId.Value)).Contains(id))
            return Forbid();

        if (string.IsNullOrWhiteSpace(request.Texto))
            return BadRequest(new { message = "El texto de la observación es obligatorio." });

        var observacion = new Models.DB.DocenteObservacion
        {
            StudentId = id,
            DocenteId = userId.Value,
            SchoolYearId = request.SchoolYearId,
            Texto = request.Texto.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.DocenteObservacion.Add(observacion);
        await _context.SaveChangesAsync();

        var created = (await QueryObservacionesAsync(id).ConfigureAwait(false)).FirstOrDefault(o => o.Id == observacion.Id);
        return Created($"/api/docente/alumnos/{id}/observaciones/{observacion.Id}", created);
    }

    [HttpGet("alumnos/{id:guid}/areas-atencion")]
    public async Task<IActionResult> GetStudentAttentionAreas(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!(await GetAllowedStudentIdsAsync(userId.Value)).Contains(id))
            return Forbid();

        var result = await _studentSupportRepositorie.GetStudentAttentionAreas(id);
        return Ok(result);
    }

    [HttpGet("alumnos/{id:guid}/historial")]
    public async Task<IActionResult> GetHistorial(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!(await GetAllowedStudentIdsAsync(userId.Value)).Contains(id))
            return Forbid();

        var record = await _studentRepositorie.GetStudentRecord(id);
        if (record == null)
            return NotFound();

        var disabilities = await _studentSupportRepositorie.GetStudentDisabilities(id);
        var attentionAreas = await _studentSupportRepositorie.GetStudentAttentionAreas(id);

        var actividades = await _dbConnection.QueryAsync<StudentHistorialActividadDto>("""
            SELECT
                aa.id                   AS Id,
                m.titulo                AS MaterialTitulo,
                aa.estado               AS Estado,
                a.fecha_asignacion      AS FechaAsignacion,
                a.fecha_limite          AS FechaLimite,
                aa.fecha_completado     AS FechaCompletado,
                aa.retroalimentacion    AS Retroalimentacion,
                a.instrucciones         AS Instrucciones
            FROM asignacion_alumnos aa
            JOIN asignaciones a ON a.id = aa.asignacion_id
            JOIN materiales m ON m.id = a.material_id
            WHERE aa.alumno_id = @StudentId
            ORDER BY a.fecha_asignacion DESC;
            """, new { StudentId = id });

        var observaciones = await QueryObservacionesAsync(id);

        var evaluacionesResumen = await _dbConnection.QueryAsync<EvaluacionResumenDto>("""
            SELECT 'Evaluación psicopedagógica' AS Tipo, fecha_elaboracion AS Fecha, estado AS Estado
            FROM evaluaciones_psicopedagogicas WHERE alumno_id = @StudentId
            UNION ALL
            SELECT 'Tamizaje TEA' AS Tipo, fecha AS Fecha, nivel_alerta AS Estado
            FROM tea_screenings WHERE alumno_id = @StudentId
            ORDER BY Fecha DESC;
            """, new { StudentId = id });

        return Ok(new
        {
            student = record,
            disabilities,
            attentionAreas,
            actividades,
            observaciones,
            evaluacionesResumen
        });
    }

    [HttpGet("alumnos/{id:guid}/reporte-pdf")]
    public async Task<IActionResult> GetReportePdf(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!(await GetAllowedStudentIdsAsync(userId.Value)).Contains(id))
            return Forbid();

        var record = await _studentRepositorie.GetStudentRecord(id);
        if (record == null)
            return NotFound();

        var disabilities = await _studentSupportRepositorie.GetStudentDisabilities(id);
        var attentionAreas = await _studentSupportRepositorie.GetStudentAttentionAreas(id);

        var pdfBytes = await _studentReportPdfService.GenerateAsync(record, disabilities, attentionAreas);

        return File(pdfBytes, "application/pdf", $"reporte-{id}.pdf");
    }

    private async Task<HashSet<Guid>> GetAllowedStudentIdsAsync(Guid docenteUserId)
    {
        var allowedGroupYearPairs = await _context.UserGroup
            .AsNoTracking()
            .Where(ug => ug.UserId == docenteUserId)
            .Select(ug => new { ug.GroupId, ug.SchoolYearId })
            .ToListAsync();

        if (allowedGroupYearPairs.Count == 0)
            return [];

        var groupIds = allowedGroupYearPairs.Select(g => g.GroupId).Distinct().ToArray();
        var yearIds = allowedGroupYearPairs.Select(g => g.SchoolYearId).Distinct().ToArray();

        var studentIds = await _context.Registration
            .AsNoTracking()
            .Where(r => groupIds.Contains(r.GroupId) && yearIds.Contains(r.SchoolYearId))
            .Select(r => r.StudentId)
            .ToListAsync();

        return studentIds.ToHashSet();
    }

    private Task<IEnumerable<DocenteObservacionDto>> QueryObservacionesAsync(Guid studentId) =>
        _dbConnection.QueryAsync<DocenteObservacionDto>("""
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
            """, new { StudentId = studentId });

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
