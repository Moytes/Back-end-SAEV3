using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Abstractions;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/trabajo-social")]
[Produces("application/json")]
[Authorize(Roles = "TRABAJO_SOCIAL")]
public class TrabajoSocialController(
    IUserRepositorie userRepository,
    IStudentRepositorie studentRepository,
    IAdminCatalogRepositorie catalogRepository,
    IPasswordHashService passwordHashService) : ControllerBase
{
    private readonly IUserRepositorie _userRepository = userRepository;
    private readonly IStudentRepositorie _studentRepository = studentRepository;
    private readonly IAdminCatalogRepositorie _catalogRepository = catalogRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    [HttpGet("escuelas")]
    public async Task<IActionResult> GetSchools()
    {
        var schoolIds = await GetAllowedSchoolIds();
        var allSchools = await _catalogRepository.GetSchools();
        return Ok(allSchools.Where(s => schoolIds.Contains(s.Id)));
    }

    [HttpGet("ciclos-escolares")]
    public async Task<IActionResult> GetSchoolYears([FromQuery] bool? onlyActive = null)
    {
        var years = await _catalogRepository.GetSchoolYears(onlyActive);
        return Ok(years);
    }

    [HttpGet("grados")]
    public async Task<IActionResult> GetGrades([FromQuery] int? educationLevelId = null)
    {
        var grades = await _catalogRepository.GetGrades(educationLevelId);
        return Ok(grades);
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

    [HttpPost("grupos")]
    public async Task<IActionResult> CreateGroup([FromBody] AddGroupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();
        if (!schoolIds.Contains(request.SchoolId))
            return Forbid();

        var result = await _catalogRepository.CreateGroup(request);
        if (!result.IsSuccess)
            return MapError(result.error);

        return StatusCode(201, result.Value);
    }

    [HttpGet("docentes")]
    public async Task<IActionResult> GetDocentes([FromQuery] int? schoolId = null)
    {
        var schoolIds = await GetAllowedSchoolIds();
        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return Forbid();

        if (!schoolId.HasValue)
        {
            var allDocentes = new List<Models.Dto.UserListItemDto>();
            foreach (var sid in schoolIds)
            {
                var docentes = await _userRepository.GetDocentesBySchool(sid);
                allDocentes.AddRange(docentes);
            }
            return Ok(allDocentes.DistinctBy(d => d.Id));
        }

        return Ok(await _userRepository.GetDocentesBySchool(schoolId.Value));
    }

    [HttpGet("grupos-con-docentes")]
    public async Task<IActionResult> GetGroupsWithTeachers([FromQuery] int? schoolId = null, [FromQuery] int? schoolYearId = null)
    {
        var schoolIds = await GetAllowedSchoolIds();
        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return Forbid();

        var groups = await _catalogRepository.GetGroupsWithTeachers(schoolId, schoolYearId);
        return Ok(groups.Where(g => schoolIds.Contains(g.SchoolId)));
    }

    [HttpPost("grupos/{groupId:int}/docentes")]
    public async Task<IActionResult> AssignDocenteToGroup(int groupId, [FromBody] TrabajoSocialAssignDocenteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();

        var groups = await _catalogRepository.GetGroups(null, null);
        var group = groups.FirstOrDefault(g => g.Id == groupId);
        if (group == null || !schoolIds.Contains(group.SchoolId))
            return Forbid();

        var docentes = await _userRepository.GetDocentesBySchool(group.SchoolId);
        if (!docentes.Any(d => d.Id == request.DocenteId))
            return BadRequest("El docente no pertenece a esta escuela.");

        var assignRequest = new AssignUserGroupRequest
        {
            GroupId = groupId,
            SchoolYearId = request.SchoolYearId,
            EsTitular = request.EsTitular
        };

        var result = await _userRepository.AssignUserToGroup(request.DocenteId, assignRequest);
        if (!result.IsSuccess)
            return MapError(result.error);

        return StatusCode(201, result.Value);
    }

    [HttpGet("alumnos")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search = null,
        [FromQuery] int? schoolId = null,
        [FromQuery] int? groupId = null)
    {
        var schoolIds = await GetAllowedSchoolIds();
        var students = await _studentRepository.GetStudentsBySchools(search, schoolId, groupId, schoolIds);
        return Ok(students);
    }

    [HttpPost("alumnos/registro-rapido")]
    public async Task<IActionResult> QuickRegisterStudent([FromBody] TrabajoSocialQuickStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        string? passwordHash = null;
        string? passwordSalt = null;
        string? studentPasswordHash = null;
        string? studentPasswordSalt = null;

        if (!string.IsNullOrWhiteSpace(request.TutorPassword))
        {
            passwordHash = _passwordHashService.HashPassword(request.TutorPassword, out passwordSalt);
        }

        if (!string.IsNullOrWhiteSpace(request.StudentPassword))
        {
            studentPasswordHash = _passwordHashService.HashPassword(request.StudentPassword, out studentPasswordSalt);
        }

        var schoolIds = await GetAllowedSchoolIds();
        var result = await _studentRepository.QuickRegisterStudent(
            request,
            schoolIds,
            passwordHash,
            passwordSalt,
            studentPasswordHash,
            studentPasswordSalt);

        if (!result.IsSuccess)
            return MapError(result.error);

        return StatusCode(201, result.Value);
    }

    [HttpPut("alumnos/{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();
        var belongs = await _studentRepository.StudentBelongsToSchools(id, schoolIds);
        if (!belongs)
            return NotFound(StudentErrors.StudentNotFound.Message);

        var result = await _studentRepository.UpdateStudent(id, request);
        if (!result.IsSuccess)
            return MapError(result.error);

        return Ok(new { id });
    }

    [HttpGet("alumnos/{id:guid}/tutores")]
    public async Task<IActionResult> GetTutors(Guid id)
    {
        var schoolIds = await GetAllowedSchoolIds();
        var belongs = await _studentRepository.StudentBelongsToSchools(id, schoolIds);
        if (!belongs)
            return NotFound(StudentErrors.StudentNotFound.Message);

        var tutors = await _studentRepository.GetTutorsByStudentId(id);
        return Ok(tutors);
    }

    [HttpPost("alumnos/{id:guid}/tutores")]
    public async Task<IActionResult> AddTutor(Guid id, [FromBody] AddTutorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();
        var result = await _studentRepository.AddTutorForAllowedStudent(id, request, schoolIds);
        if (!result.IsSuccess)
            return MapError(result.error);

        return StatusCode(201, result.Value);
    }

    [HttpPost("alumnos/{studentId:guid}/tutores/{tutorId:int}/cuenta")]
    public async Task<IActionResult> CreateTutorAccount(Guid studentId, int tutorId, [FromBody] TrabajoSocialTutorAccountRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();
        var passwordHash = _passwordHashService.HashPassword(request.Password, out string passwordSalt);
        var result = await _studentRepository.CreateTutorAccountForAllowedStudent(
            studentId,
            tutorId,
            request,
            schoolIds,
            passwordHash,
            passwordSalt);

        if (!result.IsSuccess)
            return MapError(result.error);

        return Ok(new { tutorId });
    }

    [HttpPost("inscripciones/masiva")]
    public async Task<IActionResult> BulkRegister([FromBody] TrabajoSocialBulkRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var schoolIds = await GetAllowedSchoolIds();
        var result = await _studentRepository.AddBulkRegistrations(request, schoolIds);
        if (!result.IsSuccess)
            return MapError(result.error);

        return StatusCode(201, result.Value);
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

    private IActionResult MapError(Error error)
    {
        if (error.Code == StudentErrors.CurpAlreadyExists.Code ||
            error.Code == GroupErrors.GroupAlreadyExists.Code ||
            error.Code == RegistrationErrors.StudentAlreadyRegisteredInSchoolYear.Code ||
            error.Code == UserErrors.EmailAlreadyExists.Code ||
            error.Code == UserErrors.UserGroupAssignmentAlreadyExists.Code)
            return Conflict(error.Message);

        if (error.Code == StudentErrors.StudentNotFound.Code ||
            error.Code == GroupErrors.GroupNotFound.Code ||
            error.Code == SchoolErrors.SchoolNotFound.Code ||
            error.Code == SchoolErrors.SchoolYearNotFound.Code ||
            error.Code == SchoolErrors.GradeNotFound.Code)
            return NotFound(error.Message);

        if (error.Code == StudentErrors.AccountCredentialsRequired.Code)
            return BadRequest(error.Message);

        return BadRequest(error.Message);
    }
}
