using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IPsychoeducationalAssessmentRepositorie
{
    Task<IEnumerable<PsychoeducationalAssessmentListItemDto>> GetAssessments(Guid? studentId, Guid? schoolYearId);
    Task<Result<Guid>> CreateAssessment(AddPsychoeducationalAssessmentRequest request);
    Task<Result<bool>> UpdateAssessment(Guid assessmentId, UpdatePsychoeducationalAssessmentRequest request);
    Task<Result<List<Guid>>> SyncBaps(Guid assessmentId, ManagePsychoBapsRequest request);
    Task<Result<List<Guid>>> SyncCollaborators(Guid assessmentId, ManagePsychoCollaboratorsRequest request);
    Task<PsychoeducationalAssessment?> GetAssessmentById(Guid assessmentId);
}