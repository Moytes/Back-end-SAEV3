using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Dto;
using Repositories.IRepositories;

namespace Controllers;

[ApiController]
[Route("api")]
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

    [HttpGet("zonas-escolares")]
    public async Task<IActionResult> GetSchoolZones()
    {
        var result = await _adminCatalogRepositorie.GetSchoolZones();
        return StandardSuccess(200, "School zones retrieved successfully", result);
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

    private static string GetDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attr = member?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }
}