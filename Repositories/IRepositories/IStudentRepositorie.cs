using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IStudentRepositorie
{
    Task<IEnumerable<StudentListItemDto>> GetStudents(string? search, int? schoolId);
    Task<Result<Guid>> CreateStudent(AddStudentRequest request);
    Task<Student?> GetStudentById(Guid studentId);
    Task<StudentRecordDto?> GetStudentRecord(Guid studentId);
    Task<Result<bool>> UpdateStudent(Guid studentId, UpdateStudentRequest request);

    Task<IEnumerable<TutorListItemDto>> GetTutorsByStudentId(Guid studentId);
    Task<Result<int>> AddTutor(Guid studentId, AddTutorRequest request);

    Task<Result<int>> AddRegistration(AddRegistrationRequest request);
    Task<IEnumerable<StudentListItemDto>> GetStudentsBySchools(string? search, int? schoolId, int? groupId, int? educationLevelId, IEnumerable<int> allowedSchoolIds);
    Task<IEnumerable<StudentListItemDto>> GetStudentsBySchoolsAndAttentionArea(string? search, int? schoolId, int? groupId, IEnumerable<int> allowedSchoolIds, int attentionAreaId);
    Task<Result<Guid>> QuickRegisterStudent(
        TrabajoSocialQuickStudentRequest request,
        IEnumerable<int> allowedSchoolIds,
        string? tutorPasswordHash,
        string? tutorPasswordSalt,
        string? studentPasswordHash,
        string? studentPasswordSalt);
    Task<Result<int>> AddBulkRegistrations(TrabajoSocialBulkRegistrationRequest request, IEnumerable<int> allowedSchoolIds);
    Task<bool> StudentBelongsToSchools(Guid studentId, IEnumerable<int> allowedSchoolIds);
    Task<Result<int>> AddTutorForAllowedStudent(Guid studentId, AddTutorRequest request, IEnumerable<int> allowedSchoolIds);
    Task<Result<int>> UpdateTutorForAllowedStudent(Guid studentId, int tutorId, AddTutorRequest request, IEnumerable<int> allowedSchoolIds);
    Task<Result<bool>> DeleteTutorForAllowedStudent(Guid studentId, int tutorId, IEnumerable<int> allowedSchoolIds);
    Task<Result<bool>> CreateTutorAccountForAllowedStudent(Guid studentId, int tutorId, TrabajoSocialTutorAccountRequest request, IEnumerable<int> allowedSchoolIds, string passwordHash, string passwordSalt);
    Task<IEnumerable<AlumnoPortalStudentDto>> GetPortalStudentsByUser(Guid userId, string roleClave);
}
