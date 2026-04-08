using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IStudentRepositorie
{
    Task<IEnumerable<StudentListItemDto>> GetStudents(string? search, Guid? schoolId);
    Task<Result<Guid>> CreateStudent(AddStudentRequest request);
    Task<Student?> GetStudentById(Guid studentId);
    Task<StudentRecordDto?> GetStudentRecord(Guid studentId);
    Task<Result<bool>> UpdateStudent(Guid studentId, UpdateStudentRequest request);

    Task<IEnumerable<TutorListItemDto>> GetTutorsByStudentId(Guid studentId);
    Task<Result<Guid>> AddTutor(Guid studentId, AddTutorRequest request);

    Task<Result<Guid>> AddRegistration(AddRegistrationRequest request);
}