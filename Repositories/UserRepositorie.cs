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

public class UserRepositorie : IUserRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public UserRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<Result<Guid>> CreateUser(AddUserRequest request, string passwordSalt, string passwordHash)
    {
        var existingUser = await _context.User
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
            return Result<Guid>.Failure(UserErrors.EmailAlreadyExists);

        if (request.SchoolZoneId.HasValue)
        {
            var schoolZoneExists = await _context.SchoolZone
                .AnyAsync(sz => sz.Id == request.SchoolZoneId.Value);

            if (!schoolZoneExists)
                return Result<Guid>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        if (request.StudentId.HasValue)
        {
            var studentExists = await _context.Student
                .AnyAsync(s => s.Id == request.StudentId.Value);

            if (!studentExists)
                return Result<Guid>.Failure(StudentErrors.StudentNotFound);

            var studentAccountExists = await _context.User
                .AnyAsync(u => u.StudentId == request.StudentId.Value);

            if (studentAccountExists)
                return Result<Guid>.Failure(UserErrors.StudentAlreadyHasUserAccount);
        }

        if (request.Role == UserRole.STUDENT && !request.StudentId.HasValue)
            return Result<Guid>.Failure(UserErrors.StudentIdRequiredForStudentRole);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            Role = request.Role,
            SchoolZoneId = request.SchoolZoneId,
            StudentId = request.StudentId,
            PhoneNumber = request.PhoneNumber,
            AvatarUrl = request.AvatarUrl,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = boolStatus.True,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(user.Id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<UserListItemDto>> GetUsers(UserRole? role, Guid? schoolZoneId, Guid? schoolId)
    {
        var sql = """
            SELECT DISTINCT
                u.id,
                u.email,
                u.name,
                u.father_last_name AS FatherLastName,
                u.mother_last_name AS MotherLastName,
                u.role,
                u.school_zone_id AS SchoolZoneId,
                u.phone_number AS PhoneNumber,
                u.avatar_url AS AvatarUrl,
                u.status,
                u.created_at AS CreatedAt,
                u.updated_at AS UpdatedAt
            FROM "user" u
            LEFT JOIN "user_school" us ON us.user_id = u.id
            WHERE (@Role IS NULL OR u.role = @Role)
              AND (@SchoolZoneId IS NULL OR u.school_zone_id = @SchoolZoneId)
              AND (@SchoolId IS NULL OR us.school_id = @SchoolId)
            ORDER BY u.name, u.father_last_name, u.mother_last_name;
            """;

        return await _dbConnection.QueryAsync<UserListItemDto>(sql, new
        {
            Role = role,
            SchoolZoneId = schoolZoneId,
            SchoolId = schoolId
        });
    }

    public async Task<Result<bool>> UpdateUser(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Result<bool>.Failure(UserErrors.UserNotFound);

        var emailInUse = await _context.User
            .AnyAsync(u => u.Email == request.Email && u.Id != userId);

        if (emailInUse)
            return Result<bool>.Failure(UserErrors.EmailAlreadyExists);

        if (request.SchoolZoneId.HasValue)
        {
            var schoolZoneExists = await _context.SchoolZone
                .AnyAsync(sz => sz.Id == request.SchoolZoneId.Value);

            if (!schoolZoneExists)
                return Result<bool>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        if (request.StudentId.HasValue)
        {
            var studentExists = await _context.Student
                .AnyAsync(s => s.Id == request.StudentId.Value);

            if (!studentExists)
                return Result<bool>.Failure(StudentErrors.StudentNotFound);

            var studentAccountExists = await _context.User
                .AnyAsync(u => u.StudentId == request.StudentId.Value && u.Id != userId);

            if (studentAccountExists)
                return Result<bool>.Failure(UserErrors.StudentAlreadyHasUserAccount);
        }

        if (request.Role == UserRole.STUDENT && !request.StudentId.HasValue)
            return Result<bool>.Failure(UserErrors.StudentIdRequiredForStudentRole);

        user.Email = request.Email;
        user.Name = request.Name;
        user.FatherLastName = request.FatherLastName;
        user.MotherLastName = request.MotherLastName;
        user.Role = request.Role;
        user.SchoolZoneId = request.SchoolZoneId;
        user.StudentId = request.StudentId;
        user.PhoneNumber = request.PhoneNumber;
        user.AvatarUrl = request.AvatarUrl;
        user.Status = request.Status;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<bool> UserExists(Guid userId)
    {
        return await _context.User.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> EmailExists(string email, Guid? excludeUserId = null)
    {
        return await _context.User
            .AnyAsync(u => u.Email == email && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
    }

    public async Task<Result<Guid>> AssignUserToGroup(Guid userId, AssignUserGroupRequest request)
    {
        var userExists = await _context.User.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var groupExists = await _context.Group.AnyAsync(g => g.Id == request.GroupId);
        if (!groupExists)
            return Result<Guid>.Failure(GroupErrors.GroupNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(sy => sy.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyAssigned = await _context.UserGroup.AnyAsync(x =>
            x.UserId == userId &&
            x.GroupId == request.GroupId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyAssigned)
            return Result<Guid>.Failure(UserErrors.UserGroupAssignmentAlreadyExists);

        var entity = new UserGroups
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GroupId = request.GroupId,
            SchoolYearId = request.SchoolYearId,
            EsTitular = request.EsTitular
        };

        await _context.UserGroup.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<Guid>> AssignUserToSchool(Guid userId, AssignUserSchoolRequest request)
    {
        var userExists = await _context.User.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var schoolExists = await _context.School.AnyAsync(s => s.Id == request.SchoolId);
        if (!schoolExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(sy => sy.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyAssigned = await _context.UserSchool.AnyAsync(x =>
            x.UserId == userId &&
            x.SchoolId == request.SchoolId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyAssigned)
            return Result<Guid>.Failure(UserErrors.UserSchoolAssignmentAlreadyExists);

        var entity = new UserSchools
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SchoolId = request.SchoolId,
            SchoolYearId = request.SchoolYearId
        };

        await _context.UserSchool.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }
}