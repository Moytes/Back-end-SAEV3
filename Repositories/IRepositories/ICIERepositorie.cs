using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface ICIERepositorie
{
    Task<IEnumerable<CIEDimensionCatalogDto>> GetDimensionCatalog();
    Task<IEnumerable<CIEEvaluationListItemDto>> GetEvaluations(Guid? studentId, Guid? schoolYearId, Guid? dimensionId);
    Task<Result<Guid>> CreateEvaluation(AddCIEEvaluationRequest request);
    Task<Result<List<Guid>>> UpsertAnswers(Guid evaluationId, UpsertCIEAnswersRequest request);
    Task<Result<List<Guid>>> UpsertPhonologyAnswers(Guid evaluationId, UpsertCIEPhonologyAnswersRequest request);
    Task<CIEEvaluation?> GetEvaluationById(Guid evaluationId);
}