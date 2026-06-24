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
    private static readonly int[] SupervisorManagedRoleIds = [3, 4, 5, 6, 7, 8];

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

        var roleExists = await _context.Role.AnyAsync(r => r.Id == request.RoleId);
        if (!roleExists)
            return Result<Guid>.Failure(UserErrors.RoleNotFound);

        if (request.SchoolZoneId.HasValue)
        {
            var schoolZoneExists = await _context.SchoolZone
                .AnyAsync(sz => sz.Id == request.SchoolZoneId.Value);

            if (!schoolZoneExists)
                return Result<Guid>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            RoleId = request.RoleId,
            SchoolZoneId = request.SchoolZoneId,
            AcademySubscriptionId = request.AcademySubscriptionId,
            Phone = request.Phone,
            AvatarUrl = request.AvatarUrl,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Activo = true,
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
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _context.User
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<UserListItemDto>> GetUsers(int? roleId, int? schoolZoneId, int? schoolId)
    {
        var sql = """
            SELECT DISTINCT
                u.id,
                u.email,
                u.name,
                u.father_last_name AS FatherLastName,
                u.mother_last_name AS MotherLastName,
                u.role_id AS RoleId,
                r.clave AS RoleClave,
                r.nombre AS RoleNombre,
                u.school_zone_id AS SchoolZoneId,
                s.id AS SchoolId,
                u.phone AS Phone,
                u.avatar_url AS AvatarUrl,
                u.activo,
                u.created_at AS CreatedAt,
                u.updated_at AS UpdatedAt,
                COALESCE(s.name, 'Acceso Global') AS SchoolName
            FROM "user" u
            INNER JOIN "role" r ON r.id = u.role_id
            LEFT JOIN "user_school" us ON us.user_id = u.id
            LEFT JOIN "school" s ON s.id = us.school_id
            WHERE (@RoleId IS NULL OR u.role_id = @RoleId)
              AND (@SchoolZoneId IS NULL OR u.school_zone_id = @SchoolZoneId)
              AND (@SchoolId IS NULL OR us.school_id = @SchoolId)
            ORDER BY u.name, u.father_last_name, u.mother_last_name;
            """;

        return await _dbConnection.QueryAsync<UserListItemDto>(sql, new
        {
            RoleId = roleId,
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

        var roleExists = await _context.Role.AnyAsync(r => r.Id == request.RoleId);
        if (!roleExists)
            return Result<bool>.Failure(UserErrors.RoleNotFound);

        if (request.SchoolZoneId.HasValue)
        {
            var schoolZoneExists = await _context.SchoolZone
                .AnyAsync(sz => sz.Id == request.SchoolZoneId.Value);

            if (!schoolZoneExists)
                return Result<bool>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        user.Email = request.Email;
        user.Name = request.Name;
        user.FatherLastName = request.FatherLastName;
        user.MotherLastName = request.MotherLastName;
        user.RoleId = request.RoleId;
        user.SchoolZoneId = request.SchoolZoneId;
        user.AcademySubscriptionId = request.AcademySubscriptionId;
        user.Phone = request.Phone;
        user.AvatarUrl = request.AvatarUrl;
        user.Activo = request.Activo;
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

    public async Task<IEnumerable<UserListItemDto>> GetDocentesBySchool(int schoolId)
    {
        var sql = """
            SELECT DISTINCT
                u.id,
                u.email,
                u.name,
                u.father_last_name AS FatherLastName,
                u.mother_last_name AS MotherLastName,
                u.role_id AS RoleId,
                r.clave AS RoleClave,
                r.nombre AS RoleNombre,
                u.phone AS Phone,
                u.activo,
                s.id AS SchoolId,
                s.name AS SchoolName
            FROM "user" u
            INNER JOIN "role" r ON r.id = u.role_id
            INNER JOIN "user_school" us ON us.user_id = u.id
            INNER JOIN "school" s ON s.id = us.school_id
            WHERE u.role_id = 8
              AND us.school_id = @SchoolId
              AND u.activo = true
            ORDER BY u.name, u.father_last_name;
            """;

        return await _dbConnection.QueryAsync<UserListItemDto>(sql, new { SchoolId = schoolId });
    }

    public async Task<Result<int>> AssignUserToGroup(Guid userId, AssignUserGroupRequest request)
    {
        var userExists = await _context.User.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Result<int>.Failure(UserErrors.UserNotFound);

        var groupExists = await _context.Group.AnyAsync(g => g.Id == request.GroupId);
        if (!groupExists)
            return Result<int>.Failure(GroupErrors.GroupNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(sy => sy.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyAssigned = await _context.UserGroup.AnyAsync(x =>
            x.UserId == userId &&
            x.GroupId == request.GroupId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyAssigned)
            return Result<int>.Failure(UserErrors.UserGroupAssignmentAlreadyExists);

        var entity = new UserGroups
        {
            UserId = userId,
            GroupId = request.GroupId,
            SchoolYearId = request.SchoolYearId,
            EsTitular = request.EsTitular
        };

        await _context.UserGroup.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<int>.Success(entity.Id);
    }

    public async Task<Result<int>> AssignUserToSchool(Guid userId, AssignUserSchoolRequest request)
    {
        var userExists = await _context.User.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return Result<int>.Failure(UserErrors.UserNotFound);

        var schoolExists = await _context.School.AnyAsync(s => s.Id == request.SchoolId);
        if (!schoolExists)
            return Result<int>.Failure(SchoolErrors.SchoolNotFound);

        var schoolYearExists = await _context.SchoolYear.AnyAsync(sy => sy.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var alreadyAssigned = await _context.UserSchool.AnyAsync(x =>
            x.UserId == userId &&
            x.SchoolId == request.SchoolId &&
            x.SchoolYearId == request.SchoolYearId);

        if (alreadyAssigned)
            return Result<int>.Failure(UserErrors.UserSchoolAssignmentAlreadyExists);

        var entity = new UserSchools
        {
            UserId = userId,
            SchoolId = request.SchoolId,
            SchoolYearId = request.SchoolYearId
        };

        await _context.UserSchool.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<int>.Success(entity.Id);
    }

    public async Task<Result<int>> AssignSupervisorToSchool(Guid userId, AssignSupervisorSchoolRequest request)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Result<int>.Failure(UserErrors.UserNotFound);

        var schoolExists = await _context.School.AnyAsync(s => s.Id == request.SchoolId);
        if (!schoolExists)
            return Result<int>.Failure(SchoolErrors.SchoolNotFound);

        var activeYear = await _context.SchoolYear
            .Where(sy => sy.Activo)
            .OrderByDescending(sy => sy.StartDate)
            .FirstOrDefaultAsync();

        if (activeYear == null)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        user.RoleId = 2;
        user.UpdatedAt = DateTime.UtcNow;

        var existingAssignment = await _context.UserSchool.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.SchoolId == request.SchoolId &&
            x.SchoolYearId == activeYear.Id);

        if (existingAssignment != null)
        {
            await _context.SaveChangesAsync();
            return Result<int>.Success(existingAssignment.Id);
        }

        var entity = new UserSchools
        {
            UserId = userId,
            SchoolId = request.SchoolId,
            SchoolYearId = activeYear.Id
        };

        await _context.UserSchool.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<int>.Success(entity.Id);
    }

    public async Task<IEnumerable<int>> GetUserSchools(Guid userId)
    {
        var user = await _context.User.FindAsync(userId);

        var explicitSchools = await _context.UserSchool
            .Where(us => us.UserId == userId)
            .Select(us => us.SchoolId)
            .ToListAsync();

        if (user?.SchoolZoneId != null)
        {
            var zoneSchools = await _context.School
                .Where(s => s.SchoolZoneId == user.SchoolZoneId)
                .Select(s => s.Id)
                .ToListAsync();

            return explicitSchools.Union(zoneSchools).Distinct().ToList();
        }

        return explicitSchools;
    }

    public async Task<IEnumerable<SchoolListItemDto>> GetSupervisorSchools(Guid supervisorId)
    {
        var schoolIds = await GetUserSchools(supervisorId);

        return await _context.School
            .AsNoTracking()
            .Include(s => s.SchoolZone)
            .Include(s => s.EducationLevel)
            .Where(s => schoolIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => new SchoolListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                CCT = s.CCT,
                Turn = (int)s.Turn,
                TurnName = s.Turn.ToString().ToUpper(),
                Address = s.Address,
                Phone = s.Phone,
                Activa = s.Activa,
                EducationLevelId = s.EducationLevelId,
                EducationLevelName = s.EducationLevel.Nombre,
                SchoolZoneId = s.SchoolZoneId,
                SchoolZoneNumber = s.SchoolZone != null ? s.SchoolZone.Number : null,
                SchoolZoneName = s.SchoolZone != null ? s.SchoolZone.Name : null
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserListItemDto>> GetSupervisorStaff(Guid supervisorId, int? roleId, int? schoolId)
    {
        var schoolIds = (await GetUserSchools(supervisorId)).ToArray();
        if (schoolIds.Length == 0)
            return [];

        if (schoolId.HasValue && !schoolIds.Contains(schoolId.Value))
            return [];

        var allowedRoles = SupervisorManagedRoleIds;

        var query =
            from user in _context.User.AsNoTracking()
            join role in _context.Role.AsNoTracking() on user.RoleId equals role.Id
            join userSchool in _context.UserSchool.AsNoTracking() on user.Id equals userSchool.UserId
            join school in _context.School.AsNoTracking() on userSchool.SchoolId equals school.Id
            where schoolIds.Contains(userSchool.SchoolId)
                  && allowedRoles.Contains(user.RoleId)
                  && (!roleId.HasValue || user.RoleId == roleId.Value)
                  && (!schoolId.HasValue || userSchool.SchoolId == schoolId.Value)
            orderby user.Name, user.FatherLastName, user.MotherLastName
            select new UserListItemDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                FatherLastName = user.FatherLastName,
                MotherLastName = user.MotherLastName,
                RoleId = user.RoleId,
                RoleClave = role.Clave,
                RoleNombre = role.Nombre,
                SchoolZoneId = user.SchoolZoneId,
                SchoolId = school.Id,
                SchoolName = school.Name,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                Activo = user.Activo,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

        return await query.ToListAsync();
    }

    public async Task<Result<Guid>> CreateSupervisorStaff(Guid supervisorId, SupervisorCreateStaffRequest request, string passwordSalt, string passwordHash)
    {
        if (!SupervisorManagedRoleIds.Contains(request.RoleId))
            return Result<Guid>.Failure(UserErrors.RoleNotAllowed);

        var canManageSchool = await CanSupervisorManageSchool(supervisorId, request.SchoolId);
        if (!canManageSchool)
            return Result<Guid>.Failure(SchoolErrors.SchoolNotFound);

        var activeYear = await GetActiveSchoolYear();
        if (activeYear == null)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var userRequest = new AddUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            RoleId = request.RoleId,
            Phone = request.Phone,
            AvatarUrl = request.AvatarUrl
        };

        var result = await CreateUser(userRequest, passwordSalt, passwordHash);
        if (!result.IsSuccess)
            return result;

        var assignment = new UserSchools
        {
            UserId = result.Value,
            SchoolId = request.SchoolId,
            SchoolYearId = activeYear.Id
        };

        await _context.UserSchool.AddAsync(assignment);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<Result<bool>> UpdateSupervisorStaff(Guid supervisorId, Guid staffId, SupervisorUpdateStaffRequest request)
    {
        if (!SupervisorManagedRoleIds.Contains(request.RoleId))
            return Result<bool>.Failure(UserErrors.RoleNotAllowed);

        var canManageSchool = await CanSupervisorManageSchool(supervisorId, request.SchoolId);
        if (!canManageSchool)
            return Result<bool>.Failure(SchoolErrors.SchoolNotFound);

        var staffBelongsToSupervisor = await StaffBelongsToSupervisor(supervisorId, staffId);
        if (!staffBelongsToSupervisor)
            return Result<bool>.Failure(UserErrors.UserNotFound);

        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == staffId);
        if (user == null || !SupervisorManagedRoleIds.Contains(user.RoleId))
            return Result<bool>.Failure(UserErrors.UserNotFound);

        var emailInUse = await _context.User.AnyAsync(u => u.Email == request.Email && u.Id != staffId);
        if (emailInUse)
            return Result<bool>.Failure(UserErrors.EmailAlreadyExists);

        user.Email = request.Email;
        user.Name = request.Name;
        user.FatherLastName = request.FatherLastName;
        user.MotherLastName = request.MotherLastName;
        user.RoleId = request.RoleId;
        user.Phone = request.Phone;
        user.AvatarUrl = request.AvatarUrl;
        user.Activo = request.Activo;
        user.UpdatedAt = DateTime.UtcNow;

        var activeYear = await GetActiveSchoolYear();
        if (activeYear == null)
            return Result<bool>.Failure(SchoolErrors.SchoolYearNotFound);

        var existingAssignment = await _context.UserSchool.FirstOrDefaultAsync(us =>
            us.UserId == staffId && us.SchoolYearId == activeYear.Id);

        if (existingAssignment == null)
        {
            await _context.UserSchool.AddAsync(new UserSchools
            {
                UserId = staffId,
                SchoolId = request.SchoolId,
                SchoolYearId = activeYear.Id
            });
        }
        else
        {
            existingAssignment.SchoolId = request.SchoolId;
        }

        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    private async Task<bool> CanSupervisorManageSchool(Guid supervisorId, int schoolId)
    {
        var schoolIds = await GetUserSchools(supervisorId);
        return schoolIds.Contains(schoolId);
    }

    private async Task<bool> StaffBelongsToSupervisor(Guid supervisorId, Guid staffId)
    {
        var schoolIds = (await GetUserSchools(supervisorId)).ToArray();
        if (schoolIds.Length == 0)
            return false;

        return await _context.UserSchool.AnyAsync(us =>
            us.UserId == staffId && schoolIds.Contains(us.SchoolId));
    }

    private Task<SchoolYear?> GetActiveSchoolYear()
    {
        return _context.SchoolYear
            .Where(sy => sy.Activo)
            .OrderByDescending(sy => sy.StartDate)
            .FirstOrDefaultAsync();
    }
}
