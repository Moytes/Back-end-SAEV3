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

public class AssignmentRepositorie : IAssignmentRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public AssignmentRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<Result<Guid>> CreateAssignment(AddAssignmentRequest request)
    {
        var materialExists = await _context.Material.AnyAsync(x => x.Id == request.MaterialId);
        if (!materialExists)
            return Result<Guid>.Failure(MaterialErrors.MaterialNotFound);

        var assignedByExists = await _context.User.AnyAsync(x => x.Id == request.AssignedById);
        if (!assignedByExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var studentIds = (request.StudentIds ?? [])
            .Distinct()
            .ToList();

        if (studentIds.Count == 0)
            return Result<Guid>.Failure(AssignmentErrors.GroupOrStudentsRequired);

        var existingStudents = await _context.Student
            .Where(x => studentIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        if (existingStudents.Count != studentIds.Count)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        await using var tx = await _context.Database.BeginTransactionAsync();

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            MaterialId = request.MaterialId,
            AssignedById = request.AssignedById,
            SchoolYearId = request.SchoolYearId,
            AssignmentDate = request.AssignedDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow,
            DueDate = request.DueDate,
            Instructions = request.Instructions,
            Active = boolStatus.True
        };

        await _context.Assignment.AddAsync(assignment);

        var rows = studentIds.Select(studentId => new AssignmentStudent
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = studentId,
            Status = assignmentStudentStatus.PENDIENTE
        }).ToList();

        await _context.AssignmentStudent.AddRangeAsync(rows);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<Guid>.Success(assignment.Id);
    }

    public async Task<IEnumerable<StudentAssignmentListItemDto>> GetAssignmentsByStudent(Guid studentId)
    {
        var sql = """
            SELECT
                ast.id AS AssignmentStudentId,
                ast.assignment_id AS AssignmentId,
                ast.student_id AS StudentId,

                m.id AS MaterialId,
                m.title AS MaterialTitle,
                m.description AS MaterialDescription,
                m.file_url AS MaterialFileUrl,
                m.thumbnail_url AS MaterialThumbnailUrl,

                a.assignment_date AS AssignedDate,
                a.due_date AS DueDate,
                ast.status,

                NULL::smallint AS ProgressPercent,
                NULL::smallint AS Score,
                ast.feedback AS TeacherNotes,
                ast.response_json AS StudentResponseJson,

                NULL::uuid AS GroupId,
                NULL::text AS GroupName,
                a.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName
            FROM "assignment_student" ast
            INNER JOIN "assignment" a ON a.id = ast.assignment_id
            INNER JOIN "material" m ON m.id = a.material_id
            INNER JOIN "school_year" sy ON sy.id = a.school_year_id
            WHERE ast.student_id = @StudentId
            ORDER BY a.assignment_date DESC;
            """;

        return await _dbConnection.QueryAsync<StudentAssignmentListItemDto>(sql, new
        {
            StudentId = studentId
        });
    }

    public async Task<Result<bool>> CompleteAssignmentStudent(Guid assignmentStudentId, CompleteAssignmentStudentRequest request)
    {
        var entity = await _context.AssignmentStudent
            .FirstOrDefaultAsync(x => x.Id == assignmentStudentId);

        if (entity == null)
            return Result<bool>.Failure(AssignmentErrors.AssignmentStudentNotFound);

        entity.Status = assignmentStudentStatus.COMPLETADO;
        entity.CompletedDate = DateTime.UtcNow;
        entity.ResponseJson = request.StudentResponseJson;
        entity.Feedback = request.TeacherNotes;

        if (request.Score.HasValue)
        {
            entity.ManualGradeJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                score = request.Score.Value
            });

            entity.EvaluationDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<Guid>> AddDialogProgress(Guid dialogId, AddDialogProgressRequest request)
    {
        var dialog = await _context.Dialog
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dialogId);

        if (dialog == null)
            return Result<Guid>.Failure(DialogErrors.DialogNotFound);

        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var assignmentStudent = await _context.AssignmentStudent
            .Include(x => x.Assignment)
            .FirstOrDefaultAsync(x => x.Id == request.AssignmentStudentId);

        if (assignmentStudent == null)
            return Result<Guid>.Failure(AssignmentErrors.AssignmentStudentNotFound);

        if (assignmentStudent.StudentId != request.StudentId)
            return Result<Guid>.Failure(DialogErrors.AssignmentStudentDoesNotBelongToStudent);

        if (assignmentStudent.Assignment.MaterialId != dialog.MaterialId)
            return Result<Guid>.Failure(DialogErrors.DialogDoesNotBelongToAssignmentMaterial);

        var progress = new DialogProgress
        {
            Id = Guid.NewGuid(),
            DialogId = dialogId,
            StudentId = request.StudentId,
            CurrentScene = request.CurrentScene ?? 0,
            ResponsesJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                chosenOption = request.ChosenOption,
                emotionDetected = request.EmotionDetected,
                voiceText = request.VoiceText
            }),
            Completed = request.Completed ?? boolStatus.False,
            StartDate = DateTime.UtcNow,
            EndDate = request.Completed == boolStatus.True ? DateTime.UtcNow : null
        };

        await _context.DialogProgress.AddAsync(progress);

        if (assignmentStudent.Status == assignmentStudentStatus.PENDIENTE)
            assignmentStudent.StartDate = DateTime.UtcNow;

        assignmentStudent.Status = progress.Completed == boolStatus.True
            ? assignmentStudentStatus.COMPLETADO
            : assignmentStudentStatus.EN_PROGRESO;

        if (progress.Completed == boolStatus.True)
            assignmentStudent.CompletedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<Guid>.Success(progress.Id);
    }

    public async Task<AssignmentStudent?> GetAssignmentStudentById(Guid assignmentStudentId)
    {
        return await _context.AssignmentStudent
            .FirstOrDefaultAsync(x => x.Id == assignmentStudentId);
    }

    public async Task<Dialog?> GetDialogById(Guid dialogId)
    {
        return await _context.Dialog
            .FirstOrDefaultAsync(x => x.Id == dialogId);
    }
}