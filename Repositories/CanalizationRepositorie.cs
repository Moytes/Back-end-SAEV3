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

public class CanalizationRepositorie : ICanalizationRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public CanalizationRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<CanalizationListItemDto>> GetCanalizations(
        canalizationStatus? status,
        Guid? requesterId,
        Guid? receiverId)
    {
        var sql = """
            SELECT
                c.id,
                c.canalization_date AS CanalizationDate,
                c.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                c.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                c.attention_area_id AS AttentionAreaId,
                aa.name AS AttentionAreaName,
                c.reason AS Reason,
                c.classroom_actions AS ClassroomActions,
                c.requester_id AS RequesterId,
                CONCAT(rq.name, ' ', rq.father_last_name, ' ', COALESCE(rq.mother_last_name, '')) AS RequesterFullName,
                c.receiver_id AS ReceiverId,
                CONCAT(rc.name, ' ', rc.father_last_name, ' ', COALESCE(rc.mother_last_name, '')) AS ReceiverFullName,
                c.received_date AS ReceivedDate,
                c.status,
                c.created_at AS CreatedAt
            FROM "canalization" c
            INNER JOIN "student" s ON s.id = c.student_id
            INNER JOIN "school_year" sy ON sy.id = c.school_year_id
            INNER JOIN "attention_area" aa ON aa.id = c.attention_area_id
            INNER JOIN "user" rq ON rq.id = c.requester_id
            INNER JOIN "user" rc ON rc.id = c.receiver_id
            WHERE (@Status IS NULL OR c.status = @Status)
              AND (@RequesterId IS NULL OR c.requester_id = @RequesterId)
              AND (@ReceiverId IS NULL OR c.receiver_id = @ReceiverId)
            ORDER BY c.created_at DESC, c.canalization_date DESC;
            """;

        return await _dbConnection.QueryAsync<CanalizationListItemDto>(sql, new
        {
            Status = status,
            RequesterId = requesterId,
            ReceiverId = receiverId
        });
    }

    public async Task<Result<Guid>> CreateCanalization(AddCanalizationRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var attentionAreaExists = await _context.AttentionArea.AnyAsync(x => x.Id == request.AttentionAreaId);
        if (!attentionAreaExists)
            return Result<Guid>.Failure(AttentionAreaErrors.AttentionAreaNotFound);

        var requester = await _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RequesterId);
        if (requester == null)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var receiver = await _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ReceiverId);
        if (receiver == null)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        if (request.RequesterId == request.ReceiverId)
            return Result<Guid>.Failure(CanalizationErrors.RequesterAndReceiverMustBeDifferent);

        var entity = new Canalization
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            SchoolYearId = request.SchoolYearId,
            AttentionAreaId = request.AttentionAreaId,
            RequesterId = request.RequesterId,
            ReceiverId = request.ReceiverId,
            CanalizationDate = request.CanalizationDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Reason = request.Reason,
            ClassroomActions = request.ClassroomActions,
            ReceivedDate = request.ReceivedDate,
            Status = request.ReceivedDate.HasValue
                ? canalizationStatus.RECIBIDA
                : canalizationStatus.PENDIENTE,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Canalization.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<bool>> UpdateCanalizationStatus(Guid canalizationId, UpdateCanalizationStatusRequest request)
    {
        var canalization = await _context.Canalization
            .FirstOrDefaultAsync(x => x.Id == canalizationId);

        if (canalization == null)
            return Result<bool>.Failure(CanalizationErrors.CanalizationNotFound);

        if (canalization.Status == canalizationStatus.CERRADA &&
            request.Status != canalizationStatus.CERRADA)
            return Result<bool>.Failure(CanalizationErrors.ClosedCanalizationCannotBeReopened);

        canalization.Status = request.Status;

        if (request.Status == canalizationStatus.RECIBIDA && !canalization.ReceivedDate.HasValue)
        {
            canalization.ReceivedDate = request.ReceivedDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        }
        else if (request.ReceivedDate.HasValue)
        {
            canalization.ReceivedDate = request.ReceivedDate;
        }

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Canalization?> GetCanalizationById(Guid canalizationId)
    {
        return await _context.Canalization
            .FirstOrDefaultAsync(x => x.Id == canalizationId);
    }
}