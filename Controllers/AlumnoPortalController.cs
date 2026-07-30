using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Services.IServices;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/alumno-portal")]
[Authorize(Roles = "TUTOR, ALUMNO")]
public class AlumnoPortalController(
    IStudentRepositorie studentRepositorie,
    IStudentSupportRepositorie studentSupportRepositorie,
    IStudentReportPdfService studentReportPdfService) : ControllerBase
{
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
    private readonly IStudentSupportRepositorie _studentSupportRepositorie = studentSupportRepositorie;
    private readonly IStudentReportPdfService _studentReportPdfService = studentReportPdfService;

    [HttpGet("perfil")]
    public async Task<IActionResult> GetPerfil()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var roleClave = User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClave)) return Unauthorized();

        var students = await _studentRepositorie.GetPortalStudentsByUser(userId.Value, roleClave);
        var studentList = students.ToList();

        if (studentList.Count == 0)
            return NotFound(new { message = "No se encontraron alumnos vinculados a esta cuenta." });

        return Ok(new
        {
            roleClave,
            accessedByTutor = roleClave == "TUTOR",
            students = studentList
        });
    }

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetAlumnos()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var roleClave = User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClave)) return Unauthorized();

        var students = await _studentRepositorie.GetPortalStudentsByUser(userId.Value, roleClave);
        return Ok(students);
    }

    [HttpGet("alumnos/{id:guid}/historial")]
    [Authorize(Roles = "TUTOR")]
    public async Task<IActionResult> GetHistorial(Guid id)
    {
        if (!GetAllowedStudentIds().Contains(id))
            return Forbid();

        var record = await _studentRepositorie.GetStudentRecord(id);
        if (record == null)
            return NotFound();

        var disabilities = await _studentSupportRepositorie.GetStudentDisabilities(id);
        var attentionAreas = await _studentSupportRepositorie.GetStudentAttentionAreas(id);

        return Ok(new
        {
            student = record,
            disabilities,
            attentionAreas
        });
    }

    [HttpGet("alumnos/{id:guid}/reporte-pdf")]
    [Authorize(Roles = "TUTOR")]
    public async Task<IActionResult> GetReportePdf(Guid id)
    {
        if (!GetAllowedStudentIds().Contains(id))
            return Forbid();

        var record = await _studentRepositorie.GetStudentRecord(id);
        if (record == null)
            return NotFound();

        var disabilities = await _studentSupportRepositorie.GetStudentDisabilities(id);
        var attentionAreas = await _studentSupportRepositorie.GetStudentAttentionAreas(id);

        var pdfBytes = await _studentReportPdfService.GenerateAsync(record, disabilities, attentionAreas);

        return File(pdfBytes, "application/pdf", $"reporte-{id}.pdf");
    }

    private List<Guid> GetAllowedStudentIds()
    {
        return User.FindAll("student_id")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
