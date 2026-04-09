using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IMaterialRepositorie
{
    Task<IEnumerable<MaterialTypeCatalogItemDto>> GetMaterialTypes();
    Task<IEnumerable<MaterialListItemDto>> GetMaterials(string? tag, Guid? dimensionId, short? grade);
    Task<Result<Guid>> CreateMaterial(AddMaterialRequest request);
    Task<Result<List<Guid>>> AssignTags(Guid materialId, AssignMaterialTagsRequest request);
    Task<Material?> GetMaterialById(Guid materialId);

    Task<IEnumerable<DialogListItemDto>> GetDialogs(Guid? materialId = null);
    Task<Result<Guid>> CreateDialog(AddDialogRequest request);
}