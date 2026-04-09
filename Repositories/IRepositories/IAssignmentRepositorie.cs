using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IAssignmentRepositorie
{
    Task<Result<Guid>> CreateAssignment(AddAssignmentRequest request);
    Task<IEnumerable<StudentAssignmentListItemDto>> GetAssignmentsByStudent(Guid studentId);
    Task<Result<bool>> CompleteAssignmentStudent(Guid assignmentStudentId, CompleteAssignmentStudentRequest request);
    Task<Result<Guid>> AddDialogProgress(Guid dialogId, AddDialogProgressRequest request);

    Task<AssignmentStudent?> GetAssignmentStudentById(Guid assignmentStudentId);
    Task<Dialog?> GetDialogById(Guid dialogId);
}