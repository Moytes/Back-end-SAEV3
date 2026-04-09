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

public class PsychoeducationalAssessmentRepositorie : IPsychoeducationalAssessmentRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public PsychoeducationalAssessmentRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<PsychoeducationalAssessmentListItemDto>> GetAssessments(Guid? studentId, Guid? schoolYearId)
    {
        var sql = """
            SELECT
                p.id,
                p.evaluation_date AS EvaluationDate,
                p.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                p.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                p.status,
                p.created_at AS CreatedAt,
                p.updated_at AS UpdatedAt
            FROM "psychoeducational_assessment" p
            INNER JOIN "student" s ON s.id = p.student_id
            INNER JOIN "school_year" sy ON sy.id = p.school_year_id
            WHERE (@StudentId IS NULL OR p.student_id = @StudentId)
              AND (@SchoolYearId IS NULL OR p.school_year_id = @SchoolYearId)
            ORDER BY p.updated_at DESC, p.evaluation_date DESC;
            """;

        return await _dbConnection.QueryAsync<PsychoeducationalAssessmentListItemDto>(sql, new
        {
            StudentId = studentId,
            SchoolYearId = schoolYearId
        });
    }

    public async Task<Result<Guid>> CreateAssessment(AddPsychoeducationalAssessmentRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyExists = await _context.PsychoeducationalAssessment.AnyAsync(x =>
            x.StudentId == request.StudentId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyExists)
            return Result<Guid>.Failure(PsychoeducationalAssessmentErrors.AssessmentAlreadyExistsForStudentAndSchoolYear);

        var entity = new PsychoeducationalAssessment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            SchoolYearId = request.SchoolYearId,
            EvaluationDate = request.EvaluationDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Status = assessmentStatus.BORRADOR,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.PsychoeducationalAssessment.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<bool>> UpdateAssessment(Guid assessmentId, UpdatePsychoeducationalAssessmentRequest request)
    {
        var entity = await _context.PsychoeducationalAssessment
            .FirstOrDefaultAsync(x => x.Id == assessmentId);

        if (entity == null)
            return Result<bool>.Failure(PsychoeducationalAssessmentErrors.AssessmentNotFound);

        if (entity.Status == assessmentStatus.ENTREGADA)
            return Result<bool>.Failure(PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited);

        if (request.EvaluationDate.HasValue)
            entity.EvaluationDate = request.EvaluationDate.Value;

        entity.EvaluationReason = request.EvaluationReason;
        entity.EvaluationBehavior = request.EvaluationBehavior;

        entity.PregnancyHistory = request.PregnancyHistory;
        entity.HereditaryHistory = request.HereditaryHistory;
        entity.MotorDevelopment = request.MotorDevelopment;
        entity.LanguageDevelopment = request.LanguageDevelopment;
        entity.MedicalHistory = request.MedicalHistory;
        entity.SchoolHistory = request.SchoolHistory;
        entity.FamilySituation = request.FamilySituation;

        entity.StudentDescription = request.StudentDescription;
        entity.FamilyContext = request.FamilyContext;
        entity.SchoolContext = request.SchoolContext;
        entity.SocialContext = request.SocialContext;
        entity.PhysicalDevelopment = request.PhysicalDevelopment;
        entity.CognitiveDevelopment = request.CognitiveDevelopment;
        entity.SocioAffectiveDevelopment = request.SocioAffectiveDevelopment;
        entity.LearningEvaluation = request.LearningEvaluation;
        entity.Creativity = request.Creativity;

        entity.ResultsInterpretation = request.ResultsInterpretation;
        entity.Conclusions = request.Conclusions;

        if (request.Status.HasValue)
            entity.Status = request.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<Guid>>> SyncBaps(Guid assessmentId, ManagePsychoBapsRequest request)
    {
        var assessment = await _context.PsychoeducationalAssessment
            .FirstOrDefaultAsync(x => x.Id == assessmentId);

        if (assessment == null)
            return Result<List<Guid>>.Failure(PsychoeducationalAssessmentErrors.AssessmentNotFound);

        if (assessment.Status == assessmentStatus.ENTREGADA)
            return Result<List<Guid>>.Failure(PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var currentRows = await _context.PsychoBAP
            .Where(x => x.PsychoeducationalAssessmentId == assessmentId)
            .ToListAsync();

        _context.PsychoBAP.RemoveRange(currentRows);

        var newRows = request.Items.Select(x => new PsychoBAP
        {
            Id = Guid.NewGuid(),
            PsychoeducationalAssessmentId = assessmentId,
            BAPType = x.BAPType,
            Context = x.Context,
            InclusionIndicator = x.InclusionIndicator,
            Description = x.Description
        }).ToList();

        await _context.PsychoBAP.AddRangeAsync(newRows);

        assessment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Result<List<Guid>>.Success(newRows.Select(x => x.Id).ToList());
    }

    public async Task<Result<List<Guid>>> SyncCollaborators(Guid assessmentId, ManagePsychoCollaboratorsRequest request)
    {
        var assessment = await _context.PsychoeducationalAssessment
            .FirstOrDefaultAsync(x => x.Id == assessmentId);

        if (assessment == null)
            return Result<List<Guid>>.Failure(PsychoeducationalAssessmentErrors.AssessmentNotFound);

        if (assessment.Status == assessmentStatus.ENTREGADA)
            return Result<List<Guid>>.Failure(PsychoeducationalAssessmentErrors.DeliveredAssessmentCannotBeEdited);

        var internalUserIds = request.Items
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Distinct()
            .ToList();

        if (internalUserIds.Count > 0)
        {
            var existingUsers = await _context.User
                .Where(x => internalUserIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (existingUsers.Count != internalUserIds.Count)
                return Result<List<Guid>>.Failure(UserErrors.UserNotFound);
        }

        if (request.Items.Any(x => x.UserId == null && string.IsNullOrWhiteSpace(x.ExternalName)))
            return Result<List<Guid>>.Failure(PsychoeducationalAssessmentErrors.ExternalCollaboratorNameRequired);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var currentRows = await _context.PsychoCollaborator
            .Where(x => x.PsychoeducationalAssessmentId == assessmentId)
            .ToListAsync();

        _context.PsychoCollaborator.RemoveRange(currentRows);

        var newRows = request.Items.Select(x => new PsychoCollaborator
        {
            Id = Guid.NewGuid(),
            PsychoeducationalAssessmentId = assessmentId,
            UserId = x.UserId,
            ExternalName = x.ExternalName,
            CollaboratorRole = x.CollaboratorRole,
            DigitalSignature = x.DigitalSignature,
            SignatureDate = x.SignatureDate
        }).ToList();

        await _context.PsychoCollaborator.AddRangeAsync(newRows);

        assessment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Result<List<Guid>>.Success(newRows.Select(x => x.Id).ToList());
    }

    public async Task<PsychoeducationalAssessment?> GetAssessmentById(Guid assessmentId)
    {
        return await _context.PsychoeducationalAssessment
            .FirstOrDefaultAsync(x => x.Id == assessmentId);
    }
}