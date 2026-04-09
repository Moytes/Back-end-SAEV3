using System.Data;
using Dapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Models.Dto;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Abstractions;
using Utilities.Errors;

namespace Repositories;

public class CIERepositorie : ICIERepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public CIERepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<CIEDimensionCatalogDto>> GetDimensionCatalog()
    {
        var dimensions = await _context.CIEDimension
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .ToListAsync();

        var indicators = await _context.CIEIndicator
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .ToListAsync();

        var subIndicators = await _context.CIESubIndicator
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .ToListAsync();

        var result = dimensions.Select(d => new CIEDimensionCatalogDto
        {
            Id = d.Id,
            CVE = d.CVE,
            Name = d.Name,
            ColorHex = d.ColorHex,
            Description = d.Description,
            Order = d.Order,
            Indicators = indicators
                .Where(i => i.DimensionId == d.Id)
                .Select(i => new CIEIndicatorCatalogDto
                {
                    Id = i.Id,
                    DimensionId = i.DimensionId,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    Order = i.Order,
                    SubIndicators = subIndicators
                        .Where(s => s.IndicatorId == i.Id)
                        .Select(s => new CIESubIndicatorCatalogDto
                        {
                            Id = s.Id,
                            IndicatorId = s.IndicatorId,
                            Code = s.Code,
                            Name = s.Name,
                            Description = s.Description,
                            Order = s.Order
                        })
                        .ToList()
                })
                .ToList()
        });

        return result;
    }

    public async Task<IEnumerable<CIEEvaluationListItemDto>> GetEvaluations(Guid? studentId, Guid? schoolYearId, Guid? dimensionId)
    {
        var sql = """
            SELECT
                e.id,
                e.evaluation_date AS EvaluationDate,
                e.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                e.evaluator_id AS EvaluatorId,
                CONCAT(u.name, ' ', u.father_last_name, ' ', COALESCE(u.mother_last_name, '')) AS EvaluatorFullName,
                e.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                e.dimension_id AS DimensionId,
                d.name AS DimensionName,
                d.cve AS DimensionCVE,
                e.observations AS Observations,
                e.status,
                e.created_at AS CreatedAt,
                e.updated_at AS UpdatedAt
            FROM "cie_evaluation" e
            INNER JOIN "student" s ON s.id = e.student_id
            INNER JOIN "user" u ON u.id = e.evaluator_id
            INNER JOIN "school_year" sy ON sy.id = e.school_year_id
            INNER JOIN "cie_dimension" d ON d.id = e.dimension_id
            WHERE (@StudentId IS NULL OR e.student_id = @StudentId)
              AND (@SchoolYearId IS NULL OR e.school_year_id = @SchoolYearId)
              AND (@DimensionId IS NULL OR e.dimension_id = @DimensionId)
            ORDER BY e.updated_at DESC, e.evaluation_date DESC;
            """;

        return await _dbConnection.QueryAsync<CIEEvaluationListItemDto>(sql, new
        {
            StudentId = studentId,
            SchoolYearId = schoolYearId,
            DimensionId = dimensionId
        });
    }

    public async Task<Result<Guid>> CreateEvaluation(AddCIEEvaluationRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var evaluatorExists = await _context.User.AnyAsync(x => x.Id == request.EvaluatorId);
        if (!evaluatorExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var dimensionExists = await _context.CIEDimension.AnyAsync(x => x.Id == request.DimensionId);
        if (!dimensionExists)
            return Result<Guid>.Failure(CIEErrors.DimensionNotFound);

        var alreadyExists = await _context.CIEEvaluation.AnyAsync(x =>
            x.StudentId == request.StudentId &&
            x.SchoolYearId == request.SchoolYearId &&
            x.DimensionId == request.DimensionId);

        if (alreadyExists)
            return Result<Guid>.Failure(CIEErrors.EvaluationAlreadyExistsForStudentSchoolYearAndDimension);

        var entity = new CIEEvaluation
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            EvaluatorId = request.EvaluatorId,
            SchoolYearId = request.SchoolYearId,
            DimensionId = request.DimensionId,
            EvaluationDate = request.EvaluationDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Observations = request.Observations,
            Status = evaluationStatus.EN_PROCESO,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.CIEEvaluation.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<List<Guid>>> UpsertAnswers(Guid evaluationId, UpsertCIEAnswersRequest request)
    {
        var evaluation = await _context.CIEEvaluation
            .FirstOrDefaultAsync(x => x.Id == evaluationId);

        if (evaluation == null)
            return Result<List<Guid>>.Failure(CIEErrors.EvaluationNotFound);

        if (evaluation.Status == evaluationStatus.REVISADA)
            return Result<List<Guid>>.Failure(CIEErrors.ReviewedEvaluationCannotBeEdited);

        var subIndicatorIds = request.Items
            .Select(x => x.SubIndicatorId)
            .Distinct()
            .ToList();

        if (subIndicatorIds.Count != request.Items.Count)
            return Result<List<Guid>>.Failure(CIEErrors.DuplicateSubIndicatorsInRequest);

        var subIndicators = await _context.CIESubIndicator
            .Include(x => x.Indicator)
            .Where(x => subIndicatorIds.Contains(x.Id))
            .ToListAsync();

        if (subIndicators.Count != subIndicatorIds.Count)
            return Result<List<Guid>>.Failure(CIEErrors.SubIndicatorNotFound);

        if (subIndicators.Any(x => x.Indicator.DimensionId != evaluation.DimensionId))
            return Result<List<Guid>>.Failure(CIEErrors.SubIndicatorDoesNotBelongToEvaluationDimension);

        if (request.Items.Any(x => x.HelpLevel is < 0 or > 4))
            return Result<List<Guid>>.Failure(CIEErrors.InvalidHelpLevel);

        await using var tx = await _context.Database.BeginTransactionAsync();

        var existingAnswers = await _context.CIEAnswer
            .Where(x => x.EvaluationId == evaluationId && subIndicatorIds.Contains(x.SubIndicatorId))
            .ToListAsync();

        var existingBySub = existingAnswers.ToDictionary(x => x.SubIndicatorId);
        var affectedIds = new List<Guid>();

        foreach (var item in request.Items)
        {
            if (existingBySub.TryGetValue(item.SubIndicatorId, out var current))
            {
                current.Achieved = item.Achieved;
                current.HelpLevel = item.HelpLevel;
                current.ResponseType = item.ResponseType;
                current.Observation = item.Observation;
                current.EvidenceUrl = item.EvidenceUrl;
                affectedIds.Add(current.Id);
            }
            else
            {
                var newAnswer = new CIEAnswer
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = evaluationId,
                    SubIndicatorId = item.SubIndicatorId,
                    Achieved = item.Achieved,
                    HelpLevel = item.HelpLevel,
                    ResponseType = item.ResponseType,
                    Observation = item.Observation,
                    EvidenceUrl = item.EvidenceUrl
                };

                await _context.CIEAnswer.AddAsync(newAnswer);
                affectedIds.Add(newAnswer.Id);
            }
        }

        if (request.Status.HasValue)
            evaluation.Status = request.Status.Value;

        evaluation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<List<Guid>>.Success(affectedIds);
    }

    public async Task<Result<List<Guid>>> UpsertPhonologyAnswers(Guid evaluationId, UpsertCIEPhonologyAnswersRequest request)
    {
        var evaluation = await _context.CIEEvaluation
            .Include(x => x.Dimension)
            .FirstOrDefaultAsync(x => x.Id == evaluationId);

        if (evaluation == null)
            return Result<List<Guid>>.Failure(CIEErrors.EvaluationNotFound);

        if (evaluation.Status == evaluationStatus.REVISADA)
            return Result<List<Guid>>.Failure(CIEErrors.ReviewedEvaluationCannotBeEdited);

        if (!string.Equals(evaluation.Dimension.CVE, "FONOLOGIA", StringComparison.OrdinalIgnoreCase))
            return Result<List<Guid>>.Failure(CIEErrors.PhonologyAnswersRequireFonologiaDimension);

        var subIndicatorIds = request.Items
            .Select(x => x.SubIndicatorId)
            .Distinct()
            .ToList();

        if (subIndicatorIds.Count != request.Items.Count)
            return Result<List<Guid>>.Failure(CIEErrors.DuplicateSubIndicatorsInRequest);

        var subIndicators = await _context.CIESubIndicator
            .Include(x => x.Indicator)
            .Where(x => subIndicatorIds.Contains(x.Id))
            .ToListAsync();

        if (subIndicators.Count != subIndicatorIds.Count)
            return Result<List<Guid>>.Failure(CIEErrors.SubIndicatorNotFound);

        if (subIndicators.Any(x => x.Indicator.DimensionId != evaluation.DimensionId))
            return Result<List<Guid>>.Failure(CIEErrors.SubIndicatorDoesNotBelongToEvaluationDimension);

        if (subIndicators.Any(x => !string.Equals(x.Indicator.Code, "FON_APA", StringComparison.OrdinalIgnoreCase)))
            return Result<List<Guid>>.Failure(CIEErrors.PhonologyAnswersRequireFonoAparatoSubIndicators);

        await using var tx = await _context.Database.BeginTransactionAsync();

        var existingAnswers = await _context.CIEPhonologyAnswer
            .Where(x => x.EvaluationId == evaluationId && subIndicatorIds.Contains(x.SubIndicatorId))
            .ToListAsync();

        var existingBySub = existingAnswers.ToDictionary(x => x.SubIndicatorId);
        var affectedIds = new List<Guid>();

        foreach (var item in request.Items)
        {
            if (existingBySub.TryGetValue(item.SubIndicatorId, out var current))
            {
                current.Functional = item.Functional;
                current.ObservationForm = item.ObservationForm;
                affectedIds.Add(current.Id);
            }
            else
            {
                var newAnswer = new CIEPhonologyAnswer
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = evaluationId,
                    SubIndicatorId = item.SubIndicatorId,
                    Functional = item.Functional,
                    ObservationForm = item.ObservationForm
                };

                await _context.CIEPhonologyAnswer.AddAsync(newAnswer);
                affectedIds.Add(newAnswer.Id);
            }
        }

        if (request.Status.HasValue)
            evaluation.Status = request.Status.Value;

        evaluation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<List<Guid>>.Success(affectedIds);
    }

    public async Task<CIEEvaluation?> GetEvaluationById(Guid evaluationId)
    {
        return await _context.CIEEvaluation
            .FirstOrDefaultAsync(x => x.Id == evaluationId);
    }
}