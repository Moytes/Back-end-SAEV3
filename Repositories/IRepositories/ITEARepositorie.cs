using Models.Dto;
using Models.Request;
using Utilities.Abstractions;
using Models.DB;

namespace Repositories.IRepositories;

public interface ITEARepositorie
{
    Task<IEnumerable<TEAIndicatorCatalogItemDto>> GetIndicators();
    Task<IEnumerable<TEAScreeningListItemDto>> GetScreenings(Guid? studentId, Guid? schoolYearId, alertLevel? alertLevel);
    Task<Result<Guid>> CreateScreening(AddTEAScreeningRequest request);
    Task<Result<List<Guid>>> UpsertAnswers(Guid screeningId, UpsertTEAAnswersRequest request);
    Task<TEAScreening?> GetScreeningById(Guid screeningId);
}