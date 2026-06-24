using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
[Authorize]
public class StudentController(
    IStudentRepositorie studentRepositorie) : ControllerBase
{
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] int? schoolId = null)
    {
        var result = await _studentRepositorie.GetStudents(search, schoolId);
        return Ok(result);
    }

    [HttpPost("alumnos")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER,TRABAJO_SOCIAL")]
    public async Task<IActionResult> CreateStudent([FromBody] AddStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentRepositorie.CreateStudent(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.CurpAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }

    [HttpGet("alumnos/{id:guid}")]
    public async Task<IActionResult> GetStudentRecord(Guid id)
    {
        var result = await _studentRepositorie.GetStudentRecord(id);

        if (result == null)
            return NotFound(StudentErrors.StudentNotFound.Message);

        return Ok(result);
    }

    [HttpPut("alumnos/{id:guid}")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER,TRABAJO_SOCIAL")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentRepositorie.UpdateStudent(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == StudentErrors.CurpAlreadyExists.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return Ok(new { id });
    }

    [HttpGet("alumnos/{id:guid}/tutores")]
    public async Task<IActionResult> GetTutorsByStudent(Guid id)
    {
        var student = await _studentRepositorie.GetStudentById(id);
        if (student == null)
            return NotFound(StudentErrors.StudentNotFound.Message);

        var result = await _studentRepositorie.GetTutorsByStudentId(id);
        return Ok(result);
    }

    [HttpPost("alumnos/{id:guid}/tutores")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER,TRABAJO_SOCIAL")]
    public async Task<IActionResult> AddTutor(Guid id, [FromBody] AddTutorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentRepositorie.AddTutor(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }

    [HttpPost("inscripciones")]
    [Authorize(Roles = "ADMIN,DIRECTOR_USAER,ESPECIALISTA_COM,ESPECIALISTA_PSI,ESPECIALISTA_APR,TRABAJO_SOCIAL")]
    public async Task<IActionResult> AddRegistration([FromBody] AddRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _studentRepositorie.AddRegistration(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == GroupErrors.GroupNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return NotFound(result.error.Message);

            if (result.error.Code == RegistrationErrors.StudentAlreadyRegisteredInSchoolYear.Code)
                return Conflict(result.error.Message);

            return BadRequest(result.error.Message);
        }

        return StatusCode(201, result.Value);
    }
}
