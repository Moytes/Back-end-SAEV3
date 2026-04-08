using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IStudentSupportRepositorie
{
    Task<IEnumerable<DisabilityCatalogItemDto>> GetDisabilityCatalog();
    Task<IEnumerable<AttentionAreaCatalogItemDto>> GetAttentionAreasCatalog();
    Task<IEnumerable<StudentDisabilityItemDto>> GetStudentDisabilities(Guid studentId);

    Task<Result<Guid>> AddStudentDisability(Guid studentId, AddStudentDisabilityRequest request);
    Task<Result<List<Guid>>> AssignStudentAttentionAreas(Guid studentId, AssignStudentAttentionAreasRequest request);
    Task<Result<Guid>> AddAttentionMode(Guid studentId, AddAttentionModeRequest request);
}