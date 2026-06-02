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

public class StudentRepositorie : IStudentRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public StudentRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<StudentListItemDto>> GetStudents(string? search, Guid? schoolId)
    {
        var sql = """
            SELECT
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                s.Gender,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.status,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CONCAT(CAST(g.grade AS integer), '° ', g.section) AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName
            FROM "student" s
            LEFT JOIN "registration" r
                ON r.student_id = s.id
            LEFT JOIN "group" g
                ON g.id = r.group_id
            LEFT JOIN "school" sc
                ON sc.id = g.school_id
            LEFT JOIN "school_year" sy
                ON sy.id = r.school_year_id
            WHERE (
                @Search IS NULL OR
                s.name ILIKE '%' || @Search || '%' OR
                s.father_last_name ILIKE '%' || @Search || '%' OR
                s.mother_last_name ILIKE '%' || @Search || '%' OR
                s.curp ILIKE '%' || @Search || '%'
            )
            AND (
                @SchoolId IS NULL OR sc.id = @SchoolId
            )
            ORDER BY s.father_last_name, s.mother_last_name, s.name;
            """;

        return await _dbConnection.QueryAsync<StudentListItemDto>(sql, new
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            SchoolId = schoolId
        });
    }

    public async Task<Result<Guid>> CreateStudent(AddStudentRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.CURP))
        {
            var curpExists = await _context.Student
                .AnyAsync(x => x.CURP == request.CURP);

            if (curpExists)
                return Result<Guid>.Failure(StudentErrors.CurpAlreadyExists);
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            CURP = request.CURP,
            PhotoUrl = request.PhotoUrl,
            Status = BoolStatus.True,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Student.AddAsync(student);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(student.Id);
    }

    public async Task<Student?> GetStudentById(Guid studentId)
    {
        return await _context.Student
            .FirstOrDefaultAsync(x => x.Id == studentId);
    }

    public async Task<StudentRecordDto?> GetStudentRecord(Guid studentId)
    {
        var sql = """
            SELECT
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                s.Gender,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.status,
                s.created_at AS CreatedAt,
                s.updated_at AS UpdatedAt
            FROM "student" s
            WHERE s.id = @StudentId;

            SELECT
                t.id,
                t.student_id AS StudentId,
                t.complete_name AS CompleteName,
                t.parent,
                t.phone_number AS PhoneNumber,
                t.email,
                t.address
            FROM "tutor" t
            WHERE t.student_id = @StudentId
            ORDER BY t.complete_name;

            SELECT
                r.id,
                r.group_id AS GroupId,
                CONCAT(CAST(g.grade AS integer), '° ', g.section) AS GroupName,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                r.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                r.ingress_date AS IngressDate,
                r.its_new AS ItsNew,
                r.its_tracking AS ItsTracking,
                r.final_situation AS FinalSituation,
                r.notes
            FROM "registration" r
            INNER JOIN "group" g ON g.id = r.group_id
            INNER JOIN "school" sc ON sc.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = r.school_year_id
            WHERE r.student_id = @StudentId
            ORDER BY sy.start_date DESC, sc.name;
            """;

        using var multi = await _dbConnection.QueryMultipleAsync(sql, new { StudentId = studentId });

        var student = await multi.ReadFirstOrDefaultAsync<StudentRecordDto>();
        if (student == null)
            return null;

        student.Tutors = (await multi.ReadAsync<TutorListItemDto>()).ToList();
        student.Registrations = (await multi.ReadAsync<StudentRegistrationItemDto>()).ToList();

        return student;
    }

    public async Task<Result<bool>> UpdateStudent(Guid studentId, UpdateStudentRequest request)
    {
        var student = await _context.Student
            .FirstOrDefaultAsync(x => x.Id == studentId);

        if (student == null)
            return Result<bool>.Failure(StudentErrors.StudentNotFound);

        if (!string.IsNullOrWhiteSpace(request.CURP))
        {
            var curpExists = await _context.Student
                .AnyAsync(x => x.CURP == request.CURP && x.Id != studentId);

            if (curpExists)
                return Result<bool>.Failure(StudentErrors.CurpAlreadyExists);
        }

        student.Name = request.Name;
        student.FatherLastName = request.FatherLastName;
        student.MotherLastName = request.MotherLastName;
        student.Gender = request.Gender;
        student.BirthDate = request.BirthDate;
        student.CURP = request.CURP;
        student.PhotoUrl = request.PhotoUrl;
        student.Status = request.Status;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<IEnumerable<TutorListItemDto>> GetTutorsByStudentId(Guid studentId)
    {
        return await _context.Tutor
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderBy(x => x.CompleteName)
            .Select(x => new TutorListItemDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                CompleteName = x.CompleteName,
                Parent = x.Parent,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Address = x.Address
            })
            .ToListAsync();
    }

    public async Task<Result<Guid>> AddTutor(Guid studentId, AddTutorRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == studentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var tutor = new Tutor
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CompleteName = request.CompleteName,
            Parent = request.Parent,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address
        };

        await _context.Tutor.AddAsync(tutor);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(tutor.Id);
    }

    public async Task<Result<Guid>> AddRegistration(AddRegistrationRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        var groupExists = await _context.Group.AnyAsync(x => x.Id == request.GroupId);
        if (!groupExists)
            return Result<Guid>.Failure(GroupErrors.GroupNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyExists = await _context.Registration.AnyAsync(x =>
            x.StudentId == request.StudentId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyExists)
            return Result<Guid>.Failure(RegistrationErrors.StudentAlreadyRegisteredInSchoolYear);

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            GroupId = request.GroupId,
            SchoolYearId = request.SchoolYearId,
            IngressDate = request.IngressDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            ItsNew = request.ItsNew,
            ItsTracking = request.ItsTracking,
            FinalSituation = request.FinalSituation,
            Notes = request.Notes
        };

        await _context.Registration.AddAsync(registration);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(registration.Id);
    }
}