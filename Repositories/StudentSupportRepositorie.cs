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

public class StudentSupportRepositorie : IStudentSupportRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public StudentSupportRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<DisabilityCatalogItemDto>> GetDisabilityCatalog()
    {
        return await _context.Disabilitie
            .AsNoTracking()
            .OrderBy(x => x.DisabilityCategory)
            .ThenBy(x => x.Name)
            .Select(x => new DisabilityCatalogItemDto
            {
                Id = x.Id,
                CVE = x.CVE,
                Name = x.Name,
                DisabilityCategory = x.DisabilityCategory,
                Description = x.Description
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AttentionAreaCatalogItemDto>> GetAttentionAreasCatalog()
    {
        return await _context.AttentionArea
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new AttentionAreaCatalogItemDto
            {
                Id = x.Id,
                CVE = x.CVE,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentDisabilityItemDto>> GetStudentDisabilities(Guid studentId)
    {
        var sql = """
            SELECT
                sd.id,
                sd.student_id AS StudentId,
                sd.disability_id AS DisabilityId,
                d.cve AS DisabilityCVE,
                d.name AS DisabilityName,
                d.disability_category AS DisabilityCategory,
                sd.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                sd.external_diagnosis AS ExternalDiagnosis,
                sd.file_url AS FileUrl,
                sd.notes
            FROM "student_disabilitie" sd
            INNER JOIN "disabilitie" d ON d.id = sd.disability_id
            INNER JOIN "school_year" sy ON sy.id = sd.school_year_id
            WHERE sd.student_id = @StudentId
            ORDER BY sy.start_date DESC, d.disability_category, d.name;
            """;

        return await _dbConnection.QueryAsync<StudentDisabilityItemDto>(sql, new { StudentId = studentId });
    }

    public async Task<Result<Guid>> AddStudentDisability(Guid studentId, AddStudentDisabilityRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == studentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var disability = await _context.Disabilitie
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.DisabilityId);

        if (disability == null)
            return Result<Guid>.Failure(DisabilityErrors.DisabilityNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyExists = await _context.StudentDisabilitie.AnyAsync(x =>
            x.StudentId == studentId &&
            x.DisabilityId == request.DisabilityId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyExists)
            return Result<Guid>.Failure(DisabilityErrors.StudentDisabilityAlreadyExists);

        var entity = new StudentDisability
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            DisabilityId = request.DisabilityId,
            SchoolYearId = request.SchoolYearId,
            ExternalDiagnosis = request.ExternalDiagnosis,
            FileUrl = request.FileUrl,
            Notes = request.Notes
        };

        await _context.StudentDisabilitie.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<List<Guid>>> AssignStudentAttentionAreas(Guid studentId, AssignStudentAttentionAreasRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == studentId);
        if (!studentExists)
            return Result<List<Guid>>.Failure(StudentErrors.StudentNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<List<Guid>>.Failure(SchoolErrors.SchoolYearNotFound);

        var requestedAreaIds = request.Areas
            .Select(x => x.AttentionAreaId)
            .Distinct()
            .ToList();

        if (requestedAreaIds.Count != request.Areas.Count)
            return Result<List<Guid>>.Failure(AttentionAreaErrors.DuplicateAttentionAreasInRequest);

        var existingAreaIds = await _context.AttentionArea
            .Where(x => requestedAreaIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var missingAreaIds = requestedAreaIds.Except(existingAreaIds).ToList();
        if (missingAreaIds.Count > 0)
            return Result<List<Guid>>.Failure(AttentionAreaErrors.AttentionAreaNotFound);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var currentRows = await _context.StudentAttentionArea
            .Where(x => x.StudentId == studentId && x.SchoolYearId == request.SchoolYearId)
            .ToListAsync();

        var incomingByArea = request.Areas.ToDictionary(x => x.AttentionAreaId);

        var toDelete = currentRows
            .Where(x => !incomingByArea.ContainsKey(x.AttentionAreaId))
            .ToList();

        if (toDelete.Count > 0)
            _context.StudentAttentionArea.RemoveRange(toDelete);

        foreach (var row in currentRows.Where(x => incomingByArea.ContainsKey(x.AttentionAreaId)))
        {
            var item = incomingByArea[row.AttentionAreaId];
            row.IsRequired = item.IsRequired;
            row.Notes = item.Notes;
        }

        var existingIds = currentRows.Select(x => x.AttentionAreaId).ToHashSet();

        var toInsert = request.Areas
            .Where(x => !existingIds.Contains(x.AttentionAreaId))
            .Select(x => new StudentAttentionAreas
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                AttentionAreaId = x.AttentionAreaId,
                SchoolYearId = request.SchoolYearId,
                IsRequired = x.IsRequired,
                Notes = x.Notes
            })
            .ToList();

        if (toInsert.Count > 0)
            await _context.StudentAttentionArea.AddRangeAsync(toInsert);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var finalIds = currentRows
            .Where(x => incomingByArea.ContainsKey(x.AttentionAreaId))
            .Select(x => x.Id)
            .Concat(toInsert.Select(x => x.Id))
            .ToList();

        return Result<List<Guid>>.Success(finalIds);
    }

    public async Task<Result<Guid>> AddAttentionMode(Guid studentId, AddAttentionModeRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == studentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyExists = await _context.AttentionMode.AnyAsync(x =>
            x.StudentId == studentId &&
            x.SchoolYearId == request.SchoolYearId &&
            x.Phase == request.Phase &&
            x.Type == request.Type);

        if (alreadyExists)
            return Result<Guid>.Failure(AttentionModeErrors.AttentionModeAlreadyExists);

        var entity = new AttentionMode
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            SchoolYearId = request.SchoolYearId,
            Phase = request.Phase,
            Type = request.Type
        };

        await _context.AttentionMode.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }
}