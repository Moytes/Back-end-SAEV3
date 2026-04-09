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

public class TEARepositorie : ITEARepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public TEARepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<TEAIndicatorCatalogItemDto>> GetIndicators()
    {
        return await _context.TEAIndicator
            .AsNoTracking()
            .OrderBy(x => x.Domain)
            .ThenBy(x => x.Order)
            .Select(x => new TEAIndicatorCatalogItemDto
            {
                Id = x.Id,
                Domain = x.Domain,
                Code = x.Code,
                Description = x.Description,
                AgeRangeMin = x.AgeRangeMin,
                AgeRangeMax = x.AgeRangeMax,
                Order = x.Order
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<TEAScreeningListItemDto>> GetScreenings(Guid? studentId, Guid? schoolYearId, alertLevel? alertLevel)
    {
        var sql = """
            SELECT
                ts.id,
                ts.screening_date AS ScreeningDate,
                ts.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                ts.evaluator_id AS EvaluatorId,
                CONCAT(u.name, ' ', u.father_last_name, ' ', COALESCE(u.mother_last_name, '')) AS EvaluatorFullName,
                ts.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                ts.observation_context AS ObservationContext,
                ts.general_observations AS GeneralObservations,
                ts.total_score AS TotalScore,
                ts.alert_level AS AlertLevel,
                ts.requires_canalization AS RequiresCanalization,
                ts.created_at AS CreatedAt
            FROM "tea_screening" ts
            INNER JOIN "student" s ON s.id = ts.student_id
            INNER JOIN "user" u ON u.id = ts.evaluator_id
            INNER JOIN "school_year" sy ON sy.id = ts.school_year_id
            WHERE (@StudentId IS NULL OR ts.student_id = @StudentId)
              AND (@SchoolYearId IS NULL OR ts.school_year_id = @SchoolYearId)
              AND (@AlertLevel IS NULL OR ts.alert_level = @AlertLevel)
            ORDER BY ts.created_at DESC, ts.screening_date DESC;
            """;

        return await _dbConnection.QueryAsync<TEAScreeningListItemDto>(sql, new
        {
            StudentId = studentId,
            SchoolYearId = schoolYearId,
            AlertLevel = alertLevel
        });
    }

    public async Task<Result<Guid>> CreateScreening(AddTEAScreeningRequest request)
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

        var entity = new TEAScreening
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            EvaluatorId = request.EvaluatorId,
            SchoolYearId = request.SchoolYearId,
            ScreeningDate = request.ScreeningDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            ObservationContext = request.ObservationContext,
            GeneralObservations = request.GeneralObservations,
            TotalScore = 0,
            AlertLevel = alertLevel.SIN_ALERTA,
            RequiresCanalization = request.RequiresCanalization == true ? boolStatus.True : boolStatus.False,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TEAScreening.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<List<Guid>>> UpsertAnswers(Guid screeningId, UpsertTEAAnswersRequest request)
    {
        var screening = await _context.TEAScreening
            .FirstOrDefaultAsync(x => x.Id == screeningId);

        if (screening == null)
            return Result<List<Guid>>.Failure(TEAErrors.ScreeningNotFound);

        var indicatorIds = request.Items
            .Select(x => x.IndicatorId)
            .Distinct()
            .ToList();

        if (indicatorIds.Count != request.Items.Count)
            return Result<List<Guid>>.Failure(TEAErrors.DuplicateIndicatorsInRequest);

        var indicators = await _context.TEAIndicator
            .Where(x => indicatorIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        if (indicators.Count != indicatorIds.Count)
            return Result<List<Guid>>.Failure(TEAErrors.IndicatorNotFound);

        if (request.Items.Any(x => x.Frequency is < 0 or > 3))
            return Result<List<Guid>>.Failure(TEAErrors.InvalidFrequency);

        if (request.Items.Any(x => x.Intensity is < 0 or > 3))
            return Result<List<Guid>>.Failure(TEAErrors.InvalidIntensity);

        await using var tx = await _context.Database.BeginTransactionAsync();

        var existingAnswers = await _context.TEAAnswer
            .Where(x => x.ScreeningId == screeningId && indicatorIds.Contains(x.IndicatorId))
            .ToListAsync();

        var existingByIndicator = existingAnswers.ToDictionary(x => x.IndicatorId);
        var affectedIds = new List<Guid>();

        foreach (var item in request.Items)
        {
            if (existingByIndicator.TryGetValue(item.IndicatorId, out var current))
            {
                current.Frequency = item.Frequency;
                current.Intensity = item.Intensity;
                current.Observation = item.Observation;
                affectedIds.Add(current.Id);
            }
            else
            {
                var newAnswer = new TEAAnswer
                {
                    Id = Guid.NewGuid(),
                    ScreeningId = screeningId,
                    IndicatorId = item.IndicatorId,
                    Frequency = item.Frequency,
                    Intensity = item.Intensity,
                    Observation = item.Observation
                };

                await _context.TEAAnswer.AddAsync(newAnswer);
                affectedIds.Add(newAnswer.Id);
            }
        }

        // LOGICA EXACTA DEL SQL ORIGINAL:
        // total = COALESCE(SUM(frecuencia + intensidad), 0)
        var total = (short)(await _context.TEAAnswer
            .Where(x => x.ScreeningId == screeningId)
            .SumAsync(x => (x.Frequency ?? 0) + (x.Intensity ?? 0)));

        screening.TotalScore = total;
        screening.AlertLevel = total >= 30
            ? alertLevel.SIGNIFICATIVO
            : total >= 20
                ? alertLevel.MODERADO
                : total >= 10
                    ? alertLevel.LEVE
                    : alertLevel.SIN_ALERTA;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<List<Guid>>.Success(affectedIds);
    }

    public async Task<TEAScreening?> GetScreeningById(Guid screeningId)
    {
        return await _context.TEAScreening
            .FirstOrDefaultAsync(x => x.Id == screeningId);
    }
}