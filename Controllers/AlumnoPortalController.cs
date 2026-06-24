using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/alumno-portal")]
[Authorize(Roles = "TUTOR, ALUMNO")]
public class AlumnoPortalController(IStudentRepositorie studentRepositorie) : ControllerBase
{
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;

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

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
