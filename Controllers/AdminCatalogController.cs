using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/catalogos")]
[Produces("application/json")]
[Authorize]
public class AdminCatalogController(
    IAdminCatalogRepositorie adminCatalogRepositorie) : ControllerBase
{
    private readonly IAdminCatalogRepositorie _adminCatalogRepositorie = adminCatalogRepositorie;

    [HttpGet("ciclos-escolares")]
    public async Task<IActionResult> GetSchoolYears([FromQuery] bool? onlyActive = null)
    {
        var result = await _adminCatalogRepositorie.GetSchoolYears(onlyActive);
        return Ok(result);
    }

    [HttpPost("ciclos-escolares")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSchoolYear([FromBody] AddSchoolYearRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchoolYear(request);

        if (!result.IsSuccess)
            return BadRequest(result.error.Message);

        return StatusCode(201, result.Value);
    }

    [HttpGet("zonas-escolares")]
    public async Task<IActionResult> GetSchoolZones()
    {
        var result = await _adminCatalogRepositorie.GetSchoolZones();
        return Ok(result);
    }

    [HttpPost("zonas-escolares")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSchoolZone([FromBody] AddSchoolZoneRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchoolZone(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.CctAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools([FromQuery] int? schoolZoneId = null)
    {
        var result = await _adminCatalogRepositorie.GetSchools(schoolZoneId);
        return Ok(result);
    }

    [HttpPost("escuelas")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSchool([FromBody] AddSchoolRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchool(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.SchoolZoneNotFound.Code ||
                result.error.Code == SchoolErrors.EducationLevelNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == SchoolErrors.CctAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }

    [HttpGet("niveles-educativos")]
    public async Task<IActionResult> GetEducationLevels()
    {
        var result = await _adminCatalogRepositorie.GetEducationLevels();
        return Ok(result);
    }

    [HttpGet("grados")]
    public async Task<IActionResult> GetGrades([FromQuery] int? educationLevelId = null)
    {
        var result = await _adminCatalogRepositorie.GetGrades(educationLevelId);
        return Ok(result);
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> GetGroups(
        [FromQuery] int? schoolId = null,
        [FromQuery] int? schoolYearId = null)
    {
        var result = await _adminCatalogRepositorie.GetGroups(schoolId, schoolYearId);
        return Ok(result);
    }

    [HttpPost("grupos")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER,TRABAJO_SOCIAL")]
    public async Task<IActionResult> CreateGroup([FromBody] AddGroupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _adminCatalogRepositorie.CreateGroup(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.SchoolNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == SchoolErrors.GradeNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == GroupErrors.GroupAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }
}
