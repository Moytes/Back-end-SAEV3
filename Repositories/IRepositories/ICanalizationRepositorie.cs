using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface ICanalizationRepositorie
{
    Task<IEnumerable<CanalizationListItemDto>> GetCanalizations(
        canalizationStatus? status,
        Guid? requesterId,
        Guid? receiverId);

    Task<Result<Guid>> CreateCanalization(AddCanalizationRequest request);
    Task<Result<bool>> UpdateCanalizationStatus(Guid canalizationId, UpdateCanalizationStatusRequest request);
    Task<Canalization?> GetCanalizationById(Guid canalizationId);
}