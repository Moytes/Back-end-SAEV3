using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Dto;
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

    private IActionResult StandardSuccess(int httpStatusCode, string message, object? data = null)
    {
        var responseData = data switch
        {
            null => Array.Empty<object>(),
            System.Collections.IEnumerable enumerable when data is not string => enumerable.Cast<object>().ToArray(),
            _ => new[] { data }
        };

        return StatusCode(httpStatusCode, new
        {
            statusCode = httpStatusCode,
            message,
            data = responseData
        });
    }

    private IActionResult StandardError(int httpStatusCode, string message)
    {
        return StatusCode(httpStatusCode, new
        {
            statusCode = httpStatusCode,
            message,
            data = Array.Empty<object>()
        });
    }

    [HttpGet("ciclos-escolares")]
    public async Task<IActionResult> GetSchoolYears([FromQuery] bool? onlyActive = null)
    {
        var result = await _adminCatalogRepositorie.GetSchoolYears(onlyActive);
        return StandardSuccess(200, "School years retrieved successfully", result);
    }

    [HttpPost("ciclos-escolares")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public async Task<IActionResult> CreateSchoolYear([FromBody] AddSchoolYearRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchoolYear(request);

        if (!result.IsSuccess)
            return StandardError(400, result.error.Message);

        return StandardSuccess(201, "School year created successfully", result.Value);
    }

    [HttpGet("zonas-escolares")]
    public async Task<IActionResult> GetSchoolZones()
    {
        var result = await _adminCatalogRepositorie.GetSchoolZones();
        return StandardSuccess(200, "School zones retrieved successfully", result);
    }

    [HttpPost("zonas-escolares")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public async Task<IActionResult> CreateSchoolZone([FromBody] AddSchoolZoneRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchoolZone(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.CctAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        return StandardSuccess(201, "School zone created successfully", result.Value);
    }

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools([FromQuery] Guid? schoolZoneId = null)
    {
        var result = await _adminCatalogRepositorie.GetSchools(schoolZoneId);
        return StandardSuccess(200, "Schools retrieved successfully", result);
    }

    [HttpGet("grados")]
    public IActionResult GetGrades()
    {
        var gradesList = Enum.GetValues<grades>()
            .Select(g => new EnumOptionDto
            {
                Key = (int)g,
                Value = g.ToString(),
                Label = GetDisplayName(g)
            });

        return StandardSuccess(200, "Grades retrieved successfully", gradesList);
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> GetGroups(
        [FromQuery] Guid? schoolId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _adminCatalogRepositorie.GetGroups(schoolId, schoolYearId);
        return StandardSuccess(200, "Groups retrieved successfully", result);
    }

    [HttpPost("escuelas")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public async Task<IActionResult> CreateSchool([FromBody] AddSchoolRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _adminCatalogRepositorie.CreateSchool(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.SchoolZoneNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == SchoolErrors.CctAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        return StandardSuccess(201, "School created successfully", result.Value);
    }

    [HttpPost("grupos")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER")]
    public async Task<IActionResult> CreateGroup([FromBody] AddGroupRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errorMessages = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return StandardError(400, $"Errores de validación: {errorMessages}");
        }

        var result = await _adminCatalogRepositorie.CreateGroup(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == SchoolErrors.SchoolNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == GroupErrors.GroupAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        return StandardSuccess(201, "Group created successfully", result.Value);
    }

    private static string GetDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attr = member?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }
}