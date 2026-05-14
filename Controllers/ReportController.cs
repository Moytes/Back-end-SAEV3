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

    [HttpGet("sabana-datos")]
    public async Task<IActionResult> GetStudentDataSheet(
        [FromQuery] Guid? schoolId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _reportRepositorie.GetStudentDataSheet(schoolId, schoolYearId);
        return Ok(result);
    }

    [HttpGet("resumen-cie")]
    public async Task<IActionResult> GetCIESummary(
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? schoolYearId = null)
    {
        var result = await _reportRepositorie.GetCIESummary(studentId, schoolYearId);
        return Ok(result);
    }

    [HttpGet("alertas-tea")]
    public async Task<IActionResult> GetTEAAlerts(
        [FromQuery] Guid? schoolYearId = null,
        [FromQuery] alertLevel? alertLevel = null)
    {
        var result = await _reportRepositorie.GetTEAAlerts(schoolYearId, alertLevel);
        return Ok(result);
    }
}