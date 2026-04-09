using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Repositories.IRepositories;

namespace Controllers;

[ApiController]
[Route("api/reportes")]
[Produces("application/json")]
[Authorize]
public class ReportController(
    IReportRepositorie reportRepositorie) : ControllerBase
{
    private readonly IReportRepositorie _reportRepositorie = reportRepositorie;

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

    [HttpGet("sabana-datos")]
    public async Task<IActionResult> GetStudentDataSheet(
        [FromQuery] Guid? schoolId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _reportRepositorie.GetStudentDataSheet(schoolId, schoolYearId);
        return StandardSuccess(200, "Student data sheet retrieved successfully", result);
    }

    [HttpGet("resumen-cie")]
    public async Task<IActionResult> GetCIESummary(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _reportRepositorie.GetCIESummary(studentId, schoolYearId);
        return StandardSuccess(200, "CIE summary retrieved successfully", result);
    }

    [HttpGet("alertas-tea")]
    public async Task<IActionResult> GetTEAAlerts(
        [FromQuery] Guid? schoolYearId = null,
        [FromQuery] alertLevel? alertLevel = null)
    {
        var result = await _reportRepositorie.GetTEAAlerts(schoolYearId, alertLevel);
        return StandardSuccess(200, "TEA alerts retrieved successfully", result);
    }
}