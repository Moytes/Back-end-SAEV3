using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
[Authorize]
public class StudentController(
    IStudentRepositorie studentRepositorie,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IStudentRepositorie _studentRepositorie = studentRepositorie;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;

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

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] Guid? schoolId = null)
    {
        var result = await _studentRepositorie.GetStudents(search, schoolId);
        return StandardSuccess(200, "Students retrieved successfully", result);
    }

    [HttpPost("alumnos")]
    public async Task<IActionResult> CreateStudent([FromBody] AddStudentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentRepositorie.CreateStudent(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.CurpAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "student",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Student created successfully", result.Value);
    }

    [HttpGet("alumnos/{id:guid}")]
    public async Task<IActionResult> GetStudentRecord(Guid id)
    {
        var result = await _studentRepositorie.GetStudentRecord(id);

        if (result == null)
            return StandardError(404, StudentErrors.StudentNotFound.Message);

        return StandardSuccess(200, "Student record retrieved successfully", result);
    }

    [HttpPut("alumnos/{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentRepositorie.UpdateStudent(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == StudentErrors.CurpAlreadyExists.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "UPDATE",
            AffectedTable = "student",
            RecordId = id.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(200, "Student updated successfully", new { id });
    }

    [HttpGet("alumnos/{id:guid}/tutores")]
    public async Task<IActionResult> GetTutorsByStudent(Guid id)
    {
        var student = await _studentRepositorie.GetStudentById(id);
        if (student == null)
            return StandardError(404, StudentErrors.StudentNotFound.Message);

        var result = await _studentRepositorie.GetTutorsByStudentId(id);
        return StandardSuccess(200, "Tutors retrieved successfully", result);
    }

    [HttpPost("alumnos/{id:guid}/tutores")]
    public async Task<IActionResult> AddTutor(Guid id, [FromBody] AddTutorRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentRepositorie.AddTutor(id, request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code)
                return StandardError(404, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "tutor",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                StudentId = id,
                request.CompleteName,
                request.Parent,
                request.PhoneNumber,
                request.Email,
                request.Address
            })
        });

        return StandardSuccess(201, "Tutor added successfully", result.Value);
    }

    [HttpPost("inscripciones")]
    public async Task<IActionResult> AddRegistration([FromBody] AddRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return StandardError(400, "Invalid request");

        var result = await _studentRepositorie.AddRegistration(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == GroupErrors.GroupNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code)
                return StandardError(404, result.error.Message);

            if (result.error.Code == RegistrationErrors.StudentAlreadyRegisteredInSchoolYear.Code)
                return StandardError(409, result.error.Message);

            return StandardError(400, result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "registration",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StandardSuccess(201, "Registration created successfully", result.Value);
    }
}