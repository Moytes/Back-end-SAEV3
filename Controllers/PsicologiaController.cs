using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;

namespace Controllers;

[ApiController]
[Route("api/psicologia")]
[Produces("application/json")]
[Authorize(Roles = "ESPECIALISTA_PSI")]
public class PsicologiaController(
    IUserRepositorie userRepository,
    IStudentRepositorie studentRepository,
    IAdminCatalogRepositorie catalogRepository) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IStudentRepositorie _studentRepository = studentRepository;
    private readonly IAdminCatalogRepositorie _catalogRepository = catalogRepository;
    private const int PsicologiaAreaId = 2;

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools()
    {
        var schoolIds = await GetAllowedSchoolIds();
        var schools = await _catalogRepository.GetSchools();

        return Ok(schools.Where(s => schoolIds.Contains(s.Id)));
    }

    [HttpGet("ciclos-escolares")]
    public async Task<IActionResult> GetSchoolYears([FromQuery] bool? onlyActive = null)
    {
        var years = await _catalogRepository.GetSchoolYears(onlyActive);
        return Ok(years);
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> GetGroups([FromQuery] int? schoolId = null, [FromQuery] int? schoolYearId = null)
    {
        var schoolIds = await GetAllowedSchoolIds();
        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return Forbid();

        var groups = await _catalogRepository.GetGroups(schoolId, schoolYearId);
        return Ok(groups.Where(g => schoolIds.Contains(g.SchoolId)));
    }

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] int? schoolId = null,
        [FromQuery] int? groupId = null)
    {
        var schoolIds = await GetAllowedSchoolIds();
        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return Forbid();

        var students = await _studentRepository.GetStudentsBySchoolsAndAttentionArea(search, schoolId, groupId, schoolIds, PsicologiaAreaId);
        return Ok(students);
    }

    private async Task<int[]> GetAllowedSchoolIds()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return [];

        return (await _userRepository.GetUserSchools(userId.Value)).Distinct().ToArray();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
