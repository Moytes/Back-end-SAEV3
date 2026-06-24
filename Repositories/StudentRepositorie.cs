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

    public async Task<IEnumerable<StudentListItemDto>> GetStudents(string? search, int? schoolId)
    {
        var sql = """
            SELECT
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                s.sexo,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.activo,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CONCAT(gr.numero, '° ', g.section) AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName
            FROM "student" s
            LEFT JOIN "registration" r ON r.student_id = s.id
            LEFT JOIN "group" g ON g.id = r.group_id
            LEFT JOIN "grade" gr ON gr.id = g.grade_id
            LEFT JOIN "school" sc ON sc.id = g.school_id
            LEFT JOIN "school_year" sy ON sy.id = r.school_year_id
            WHERE (
                @Search IS NULL OR
                s.name ILIKE '%' || @Search || '%' OR
                s.father_last_name ILIKE '%' || @Search || '%' OR
                s.mother_last_name ILIKE '%' || @Search || '%' OR
                s.curp ILIKE '%' || @Search || '%'
            )
            AND (@SchoolId IS NULL OR sc.id = @SchoolId)
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
            Sexo = request.Sexo,
            BirthDate = request.BirthDate,
            CURP = request.CURP,
            PhotoUrl = request.PhotoUrl,
            Activo = true,
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
                s.sexo,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.activo,
                s.created_at AS CreatedAt,
                s.updated_at AS UpdatedAt
            FROM "student" s
            WHERE s.id = @StudentId;

            SELECT
                t.id,
                t.student_id AS StudentId,
                t.complete_name AS CompleteName,
                t.parentesco,
                t.phone,
                t.email,
                t.address,
                t.user_id AS UserId
            FROM "tutor" t
            WHERE t.student_id = @StudentId
            ORDER BY t.complete_name;

            SELECT
                r.id,
                r.group_id AS GroupId,
                CONCAT(gr.numero, '° ', g.section) AS GroupName,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                r.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                r.ingress_date AS IngressDate,
                r.is_new AS IsNew,
                r.is_tracking AS IsTracking,
                r.final_situation AS FinalSituation,
                r.notes
            FROM "registration" r
            INNER JOIN "group" g ON g.id = r.group_id
            INNER JOIN "grade" gr ON gr.id = g.grade_id
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
        student.Sexo = request.Sexo;
        student.BirthDate = request.BirthDate;
        student.CURP = request.CURP;
        student.PhotoUrl = request.PhotoUrl;
        student.Activo = request.Activo;
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
                Parentesco = x.Parentesco,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                UserId = x.UserId
            })
            .ToListAsync();
    }

    public async Task<Result<int>> AddTutor(Guid studentId, AddTutorRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == studentId);
        if (!studentExists)
            return Result<int>.Failure(StudentErrors.StudentNotFound);

        var tutor = new Tutor
        {
            StudentId = studentId,
            CompleteName = request.CompleteName,
            Parentesco = request.Parentesco,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            UserId = request.UserId
        };

        await _context.Tutor.AddAsync(tutor);
        await _context.SaveChangesAsync();

        return Result<int>.Success(tutor.Id);
    }

    public async Task<Result<int>> AddRegistration(AddRegistrationRequest request)
    {
        var studentExists = await _context.Student.AnyAsync(x => x.Id == request.StudentId);
        if (!studentExists)
            return Result<int>.Failure(StudentErrors.StudentNotFound);

        var groupExists = await _context.Group.AnyAsync(x => x.Id == request.GroupId);
        if (!groupExists)
            return Result<int>.Failure(GroupErrors.GroupNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyExists = await _context.Registration.AnyAsync(x =>
            x.StudentId == request.StudentId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyExists)
            return Result<int>.Failure(RegistrationErrors.StudentAlreadyRegisteredInSchoolYear);

        var registration = new Registration
        {
            StudentId = request.StudentId,
            GroupId = request.GroupId,
            SchoolYearId = request.SchoolYearId,
            IngressDate = request.IngressDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            IsNew = request.IsNew,
            IsTracking = request.IsTracking,
            FinalSituation = request.FinalSituation,
            Notes = request.Notes
        };

        await _context.Registration.AddAsync(registration);
        await _context.SaveChangesAsync();

        return Result<int>.Success(registration.Id);
    }

    public async Task<IEnumerable<StudentListItemDto>> GetStudentsBySchools(string? search, int? schoolId, int? groupId, IEnumerable<int> allowedSchoolIds)
    {
        var schoolIds = allowedSchoolIds.Distinct().ToArray();
        if (schoolIds.Length == 0)
            return [];

        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return [];

        var sql = """
            SELECT
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                s.sexo,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.activo,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CONCAT(gr.numero, '° ', g.section) AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName
            FROM "student" s
            INNER JOIN "registration" r ON r.student_id = s.id
            INNER JOIN "group" g ON g.id = r.group_id
            INNER JOIN "grade" gr ON gr.id = g.grade_id
            INNER JOIN "school" sc ON sc.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = r.school_year_id
            WHERE sc.id = ANY(@AllowedSchoolIds)
              AND (@SchoolId IS NULL OR sc.id = @SchoolId)
              AND (@GroupId IS NULL OR g.id = @GroupId)
              AND (
                @Search IS NULL OR
                s.name ILIKE '%' || @Search || '%' OR
                s.father_last_name ILIKE '%' || @Search || '%' OR
                s.mother_last_name ILIKE '%' || @Search || '%' OR
                s.curp ILIKE '%' || @Search || '%'
              )
            ORDER BY s.father_last_name, s.mother_last_name, s.name;
            """;

        return await _dbConnection.QueryAsync<StudentListItemDto>(sql, new
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            SchoolId = schoolId,
            GroupId = groupId,
            AllowedSchoolIds = schoolIds
        });
    }

    public async Task<IEnumerable<StudentListItemDto>> GetStudentsBySchoolsAndAttentionArea(
        string? search,
        int? schoolId,
        int? groupId,
        IEnumerable<int> allowedSchoolIds,
        int attentionAreaId)
    {
        var schoolIds = allowedSchoolIds.Distinct().ToArray();
        if (schoolIds.Length == 0)
            return [];

        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return [];

        var sql = """
            SELECT
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                s.sexo,
                s.birth_date AS BirthDate,
                s.curp,
                s.photo_url AS PhotoUrl,
                s.activo,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CONCAT(gr.numero, '° ', g.section) AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName
            FROM "student" s
            INNER JOIN "registration" r ON r.student_id = s.id
            INNER JOIN "group" g ON g.id = r.group_id
            INNER JOIN "grade" gr ON gr.id = g.grade_id
            INNER JOIN "school" sc ON sc.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = r.school_year_id
            WHERE sc.id = ANY(@AllowedSchoolIds)
              AND (@SchoolId IS NULL OR sc.id = @SchoolId)
              AND (@GroupId IS NULL OR g.id = @GroupId)
              AND EXISTS (
                SELECT 1
                FROM "student_attention_area" saa
                WHERE saa.student_id = s.id
                  AND saa.attention_area_id = @AttentionAreaId
                  AND saa.school_year_id = r.school_year_id
              )
              AND (
                @Search IS NULL OR
                s.name ILIKE '%' || @Search || '%' OR
                s.father_last_name ILIKE '%' || @Search || '%' OR
                s.mother_last_name ILIKE '%' || @Search || '%' OR
                s.curp ILIKE '%' || @Search || '%'
              )
            ORDER BY s.father_last_name, s.mother_last_name, s.name;
            """;

        return await _dbConnection.QueryAsync<StudentListItemDto>(sql, new
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            SchoolId = schoolId,
            GroupId = groupId,
            AllowedSchoolIds = schoolIds,
            AttentionAreaId = attentionAreaId
        });
    }

    public async Task<Result<Guid>> QuickRegisterStudent(
        TrabajoSocialQuickStudentRequest request,
        IEnumerable<int> allowedSchoolIds,
        string? tutorPasswordHash,
        string? tutorPasswordSalt,
        string? studentPasswordHash,
        string? studentPasswordSalt)
    {
        var schoolIds = allowedSchoolIds.Distinct().ToArray();
        var group = await _context.Group
            .Include(g => g.School)
            .Include(g => g.Grade)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId);

        if (group == null)
            return Result<Guid>.Failure(GroupErrors.GroupNotFound);

        if (!schoolIds.Contains(group.SchoolId))
            return Result<Guid>.Failure(SchoolErrors.SchoolNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var requiresTutorAccount = group.Grade.EducationLevelId is 1 or 2;
        var requiresStudentAccount = group.Grade.EducationLevelId is 3 or 4;

        if (requiresTutorAccount)
        {
            if (string.IsNullOrWhiteSpace(request.TutorCompleteName) ||
                string.IsNullOrWhiteSpace(request.TutorEmail) ||
                string.IsNullOrWhiteSpace(tutorPasswordHash) ||
                string.IsNullOrWhiteSpace(tutorPasswordSalt))
            {
                return Result<Guid>.Failure(StudentErrors.AccountCredentialsRequired);
            }

            var tutorEmailInUse = await _context.User.AnyAsync(u => u.Email == request.TutorEmail);
            if (tutorEmailInUse)
                return Result<Guid>.Failure(UserErrors.EmailAlreadyExists);
        }

        if (requiresStudentAccount)
        {
            if (string.IsNullOrWhiteSpace(request.StudentEmail) ||
                string.IsNullOrWhiteSpace(studentPasswordHash) ||
                string.IsNullOrWhiteSpace(studentPasswordSalt))
            {
                return Result<Guid>.Failure(StudentErrors.AccountCredentialsRequired);
            }

            var studentEmailInUse = await _context.User.AnyAsync(u => u.Email == request.StudentEmail);
            if (studentEmailInUse)
                return Result<Guid>.Failure(UserErrors.EmailAlreadyExists);
        }

        if (!string.IsNullOrWhiteSpace(request.CURP))
        {
            var curpExists = await _context.Student.AnyAsync(x => x.CURP == request.CURP);
            if (curpExists)
                return Result<Guid>.Failure(StudentErrors.CurpAlreadyExists);
        }

        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                FatherLastName = request.FatherLastName,
                MotherLastName = request.MotherLastName,
                Sexo = request.Sexo,
                BirthDate = request.BirthDate,
                CURP = request.CURP,
                PhotoUrl = request.PhotoUrl,
                Activo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Student.AddAsync(student);

            if (requiresStudentAccount)
            {
                var studentUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.StudentEmail!,
                    Name = request.Name,
                    FatherLastName = request.FatherLastName,
                    MotherLastName = request.MotherLastName,
                    RoleId = 10,
                    PasswordHash = studentPasswordHash!,
                    PasswordSalt = studentPasswordSalt!,
                    Activo = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                student.UserId = studentUser.Id;
                await _context.User.AddAsync(studentUser);
            }

            await _context.Registration.AddAsync(new Registration
            {
                StudentId = student.Id,
                GroupId = request.GroupId,
                SchoolYearId = request.SchoolYearId,
                IngressDate = request.IngressDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                IsNew = request.IsNew,
                IsTracking = request.IsTracking,
                FinalSituation = request.FinalSituation,
                Notes = request.Notes
            });

            User? tutorUser = null;
            if (requiresTutorAccount)
            {
                tutorUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.TutorEmail!,
                    Name = request.TutorCompleteName!,
                    FatherLastName = "Tutor",
                    RoleId = 9,
                    Phone = request.TutorPhone,
                    PasswordHash = tutorPasswordHash!,
                    PasswordSalt = tutorPasswordSalt!,
                    Activo = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.User.AddAsync(tutorUser);
            }

            if (!string.IsNullOrWhiteSpace(request.TutorCompleteName))
            {
                await _context.Tutor.AddAsync(new Tutor
                {
                    StudentId = student.Id,
                    CompleteName = request.TutorCompleteName,
                    Parentesco = request.TutorParentesco,
                    Phone = request.TutorPhone,
                    Email = request.TutorEmail,
                    Address = request.TutorAddress,
                    UserId = tutorUser?.Id
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<Guid>.Success(student.Id);
        });
    }

    public async Task<Result<int>> AddBulkRegistrations(TrabajoSocialBulkRegistrationRequest request, IEnumerable<int> allowedSchoolIds)
    {
        var schoolIds = allowedSchoolIds.Distinct().ToArray();
        var group = await _context.Group.FirstOrDefaultAsync(g => g.Id == request.GroupId);
        if (group == null)
            return Result<int>.Failure(GroupErrors.GroupNotFound);

        if (!schoolIds.Contains(group.SchoolId))
            return Result<int>.Failure(SchoolErrors.SchoolNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var allowedStudentIds = await _context.Registration
            .Include(r => r.Group)
            .Where(r => request.StudentIds.Contains(r.StudentId) && schoolIds.Contains(r.Group.SchoolId))
            .Select(r => r.StudentId)
            .Distinct()
            .ToListAsync();

        var newStudentIds = request.StudentIds.Where(id => allowedStudentIds.Contains(id)).Distinct().ToArray();
        if (newStudentIds.Length == 0)
            return Result<int>.Failure(StudentErrors.StudentNotFound);

        var alreadyRegistered = await _context.Registration
            .Where(r => newStudentIds.Contains(r.StudentId) && r.SchoolYearId == request.SchoolYearId)
            .Select(r => r.StudentId)
            .ToListAsync();

        var toInsert = newStudentIds.Except(alreadyRegistered).Select(studentId => new Registration
        {
            StudentId = studentId,
            GroupId = request.GroupId,
            SchoolYearId = request.SchoolYearId,
            IngressDate = request.IngressDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            IsNew = request.IsNew,
            IsTracking = request.IsTracking,
            Notes = request.Notes
        }).ToList();

        if (toInsert.Count == 0)
            return Result<int>.Failure(RegistrationErrors.StudentAlreadyRegisteredInSchoolYear);

        await _context.Registration.AddRangeAsync(toInsert);
        await _context.SaveChangesAsync();

        return Result<int>.Success(toInsert.Count);
    }

    public async Task<bool> StudentBelongsToSchools(Guid studentId, IEnumerable<int> allowedSchoolIds)
    {
        var schoolIds = allowedSchoolIds.Distinct().ToArray();
        if (schoolIds.Length == 0)
            return false;

        return await _context.Registration
            .Include(r => r.Group)
            .AnyAsync(r => r.StudentId == studentId && schoolIds.Contains(r.Group.SchoolId));
    }

    public async Task<Result<int>> AddTutorForAllowedStudent(Guid studentId, AddTutorRequest request, IEnumerable<int> allowedSchoolIds)
    {
        var belongs = await StudentBelongsToSchools(studentId, allowedSchoolIds);
        if (!belongs)
            return Result<int>.Failure(StudentErrors.StudentNotFound);

        return await AddTutor(studentId, request);
    }

    public async Task<Result<bool>> CreateTutorAccountForAllowedStudent(Guid studentId, int tutorId, TrabajoSocialTutorAccountRequest request, IEnumerable<int> allowedSchoolIds, string passwordHash, string passwordSalt)
    {
        var belongs = await StudentBelongsToSchools(studentId, allowedSchoolIds);
        if (!belongs)
            return Result<bool>.Failure(StudentErrors.StudentNotFound);

        var tutor = await _context.Tutor.FirstOrDefaultAsync(t => t.Id == tutorId && t.StudentId == studentId);
        if (tutor == null)
            return Result<bool>.Failure(StudentErrors.StudentNotFound);

        var emailInUse = await _context.User.AnyAsync(u => u.Email == request.Email);
        if (emailInUse)
            return Result<bool>.Failure(UserErrors.EmailAlreadyExists);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = tutor.CompleteName,
            FatherLastName = "Tutor",
            RoleId = 9,
            Phone = tutor.Phone,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.User.AddAsync(user);
        tutor.UserId = user.Id;
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<IEnumerable<AlumnoPortalStudentDto>> GetPortalStudentsByUser(Guid userId, string roleClave)
    {
        var accessedByTutor = roleClave == "TUTOR";
        var sql = """
            SELECT DISTINCT ON (s.id)
                s.id,
                s.name,
                s.father_last_name AS FatherLastName,
                s.mother_last_name AS MotherLastName,
                CONCAT(s.name, ' ', s.father_last_name, COALESCE(' ' || s.mother_last_name, '')) AS FullName,
                s.birth_date AS BirthDate,
                s.photo_url AS PhotoUrl,
                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CONCAT(gr.numero, '° ', g.section) AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName,
                el.id AS EducationLevelId,
                el.nombre AS EducationLevelName,
                @AccessedByTutor AS AccessedByTutor
            FROM "student" s
            INNER JOIN "registration" r ON r.student_id = s.id
            INNER JOIN "group" g ON g.id = r.group_id
            INNER JOIN "grade" gr ON gr.id = g.grade_id
            INNER JOIN "education_level" el ON el.id = gr.education_level_id
            INNER JOIN "school" sc ON sc.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = r.school_year_id
            LEFT JOIN "tutor" t ON t.student_id = s.id
            WHERE s.activo = TRUE
              AND (
                (@RoleClave = 'ALUMNO' AND s.user_id = @UserId)
                OR (@RoleClave = 'TUTOR' AND t.user_id = @UserId)
              )
            ORDER BY s.id, sy.start_date DESC, r.id DESC;
            """;

        return await _dbConnection.QueryAsync<AlumnoPortalStudentDto>(sql, new
        {
            UserId = userId,
            RoleClave = roleClave,
            AccessedByTutor = accessedByTutor
        });
    }
}
