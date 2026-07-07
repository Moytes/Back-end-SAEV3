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

public class AdminCatalogRepositorie : IAdminCatalogRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public AdminCatalogRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<SchoolYearListItemDto>> GetSchoolYears(bool? onlyActive = null)
    {
        var query = _context.SchoolYear.AsNoTracking();

        if (onlyActive.HasValue)
            query = query.Where(x => x.Activo == onlyActive.Value);

        return await query
            .OrderByDescending(x => x.Activo)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new SchoolYearListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Activo = x.Activo
            })
            .ToListAsync();
    }

    public async Task<Result<int>> CreateSchoolYear(AddSchoolYearRequest request)
    {
        var nameExists = await _context.SchoolYear.AnyAsync(x => x.Name == request.Name);
        if (nameExists)
            return Result<int>.Failure(new Error("SchoolYear.DuplicateName", "Ya existe un ciclo escolar con ese nombre."));

        if (request.Activo)
        {
            var activeYears = await _context.SchoolYear.Where(x => x.Activo).ToListAsync();
            foreach (var year in activeYears)
                year.Activo = false;
        }

        var schoolYear = new SchoolYear
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Activo = request.Activo
        };

        await _context.SchoolYear.AddAsync(schoolYear);
        await _context.SaveChangesAsync();

        return Result<int>.Success(schoolYear.Id);
    }

    public async Task<IEnumerable<SchoolZoneListItemDto>> GetSchoolZones()
    {
        return await _context.SchoolZone
            .AsNoTracking()
            .OrderBy(x => x.Number)
            .Select(x => new SchoolZoneListItemDto
            {
                Id = x.Id,
                Number = x.Number,
                CCT = x.CCT,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync();
    }

    public async Task<Result<int>> CreateSchoolZone(AddSchoolZoneRequest request)
    {
        var cctExists = await _context.SchoolZone.AnyAsync(x => x.CCT == request.CCT);
        if (cctExists)
            return Result<int>.Failure(SchoolErrors.CctAlreadyExists);

        var zone = new SchoolZone
        {
            Number = request.Number,
            CCT = request.CCT,
            Name = request.Name,
            Description = request.Description,
            AcademySubscriptionId = request.AcademySubscriptionId
        };

        await _context.SchoolZone.AddAsync(zone);
        await _context.SaveChangesAsync();

        return Result<int>.Success(zone.Id);
    }

    public async Task<IEnumerable<SchoolListItemDto>> GetSchools(int? schoolZoneId = null)
    {
        return await _context.School
            .AsNoTracking()
            .Include(s => s.SchoolZone)
            .Include(s => s.EducationLevel)
            .Where(s => !schoolZoneId.HasValue || s.SchoolZoneId == schoolZoneId)
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

    public async Task<Result<int>> CreateSchool(AddSchoolRequest request)
    {
        var levelExists = await _context.EducationLevel.AnyAsync(x => x.Id == request.EducationLevelId);
        if (!levelExists)
            return Result<int>.Failure(SchoolErrors.EducationLevelNotFound);

        if (request.SchoolZoneId.HasValue)
        {
            var zoneExists = await _context.SchoolZone.AnyAsync(x => x.Id == request.SchoolZoneId.Value);
            if (!zoneExists)
                return Result<int>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        if (!string.IsNullOrWhiteSpace(request.CCT))
        {
            var cctExists = await _context.School.AnyAsync(x => x.CCT == request.CCT);
            if (cctExists)
                return Result<int>.Failure(SchoolErrors.CctAlreadyExists);
        }

        var school = new School
        {
            Name = request.Name,
            CCT = request.CCT,
            Turn = request.Turn,
            Address = request.Address,
            Phone = request.Phone,
            EducationLevelId = request.EducationLevelId,
            SchoolZoneId = request.SchoolZoneId,
            AcademySubscriptionId = request.AcademySubscriptionId,
            Activa = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.School.AddAsync(school);
        await _context.SaveChangesAsync();

        return Result<int>.Success(school.Id);
    }

    public async Task<IEnumerable<GroupListItemDto>> GetGroups(int? schoolId = null, int? schoolYearId = null)
    {
        return await _context.Group
            .AsNoTracking()
            .Include(g => g.School)
            .Include(g => g.Grade).ThenInclude(gr => gr.EducationLevel)
            .Include(g => g.SchoolYear)
            .Where(g => (!schoolId.HasValue || g.SchoolId == schoolId) &&
                        (!schoolYearId.HasValue || g.SchoolYearId == schoolYearId))
            .OrderByDescending(g => g.SchoolYear.Name)
            .ThenBy(g => g.School.Name)
            .ThenBy(g => g.Grade.EducationLevel.Orden)
            .ThenBy(g => g.Grade.Numero)
            .ThenBy(g => g.Section)
            .Select(g => new GroupListItemDto
            {
                Id = g.Id,
                SchoolId = g.SchoolId,
                SchoolName = g.School.Name,
                SchoolCCT = g.School.CCT,
                SchoolYearId = g.SchoolYearId,
                SchoolYearName = g.SchoolYear.Name,
                GradeId = g.GradeId,
                GradeName = g.Grade.Nombre,
                GradeNumber = g.Grade.Numero,
                EducationLevelId = g.Grade.EducationLevelId,
                EducationLevelName = g.Grade.EducationLevel.Nombre,
                Section = g.Section,
                DisplayName = $"{g.Grade.Numero}° {g.Section}"
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupWithTeachersDto>> GetGroupsWithTeachers(int? schoolId = null, int? schoolYearId = null)
    {
        var groups = (await GetGroups(schoolId, schoolYearId)).ToList();
        var groupIds = groups.Select(g => g.Id).ToArray();
        if (groupIds.Length == 0) return [];

        var sql = """
            SELECT
                ug.group_id AS GroupId,
                u.id AS UserId,
                CONCAT(u.name, ' ', u.father_last_name, COALESCE(' ' || u.mother_last_name, '')) AS FullName,
                ug.es_titular AS EsTitular
            FROM "user_group" ug
            INNER JOIN "user" u ON u.id = ug.user_id
            WHERE ug.group_id = ANY(@GroupIds)
              AND u.role_id = 8
            ORDER BY ug.es_titular DESC, u.name;
            """;

        var teacherRows = (await _dbConnection.QueryAsync<(int GroupId, Guid UserId, string FullName, bool EsTitular)>(sql, new { GroupIds = groupIds })).ToList();

        return groups.Select(g => new GroupWithTeachersDto
        {
            Id = g.Id,
            SchoolId = g.SchoolId,
            SchoolName = g.SchoolName,
            SchoolCCT = g.SchoolCCT,
            SchoolYearId = g.SchoolYearId,
            SchoolYearName = g.SchoolYearName,
            GradeId = g.GradeId,
            GradeName = g.GradeName,
            GradeNumber = g.GradeNumber,
            EducationLevelId = g.EducationLevelId,
            EducationLevelName = g.EducationLevelName,
            Section = g.Section,
            DisplayName = g.DisplayName,
            Teachers = teacherRows
                .Where(t => t.GroupId == g.Id)
                .Select(t => new GroupTeacherDto { UserId = t.UserId, FullName = t.FullName, EsTitular = t.EsTitular })
                .ToList()
        });
    }

    public async Task<Result<int>> CreateGroup(AddGroupRequest request)
    {
        var schoolExists = await _context.School.AnyAsync(x => x.Id == request.SchoolId);
        if (!schoolExists)
            return Result<int>.Failure(SchoolErrors.SchoolNotFound);

        var gradeExists = await _context.Grade.AnyAsync(x => x.Id == request.GradeId);
        if (!gradeExists)
            return Result<int>.Failure(SchoolErrors.GradeNotFound);

        var yearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!yearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var groupExists = await _context.Group.AnyAsync(x =>
            x.SchoolId == request.SchoolId &&
            x.GradeId == request.GradeId &&
            x.Section == request.Section &&
            x.SchoolYearId == request.SchoolYearId);

        if (groupExists)
            return Result<int>.Failure(GroupErrors.GroupAlreadyExists);

        var group = new Group
        {
            SchoolId = request.SchoolId,
            GradeId = request.GradeId,
            Section = request.Section,
            SchoolYearId = request.SchoolYearId
        };

        await _context.Group.AddAsync(group);
        await _context.SaveChangesAsync();

        return Result<int>.Success(group.Id);
    }

    public async Task<Result<int>> UpdateGroup(int groupId, AddGroupRequest request)
    {
        var group = await _context.Group.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            return Result<int>.Failure(GroupErrors.GroupNotFound);

        var gradeExists = await _context.Grade.AnyAsync(x => x.Id == request.GradeId);
        if (!gradeExists)
            return Result<int>.Failure(SchoolErrors.GradeNotFound);

        var yearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!yearExists)
            return Result<int>.Failure(SchoolErrors.SchoolYearNotFound);

        var duplicate = await _context.Group.AnyAsync(x =>
            x.Id != groupId &&
            x.SchoolId == group.SchoolId &&
            x.GradeId == request.GradeId &&
            x.Section == request.Section &&
            x.SchoolYearId == request.SchoolYearId);

        if (duplicate)
            return Result<int>.Failure(GroupErrors.GroupAlreadyExists);

        group.GradeId = request.GradeId;
        group.Section = request.Section;
        group.SchoolYearId = request.SchoolYearId;

        await _context.SaveChangesAsync();

        return Result<int>.Success(group.Id);
    }

    public async Task<Result<int>> DeleteGroup(int groupId)
    {
        var group = await _context.Group.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            return Result<int>.Failure(GroupErrors.GroupNotFound);

        var hasStudents = await _context.Registration.AnyAsync(r => r.GroupId == groupId);
        if (hasStudents)
            return Result<int>.Failure(GroupErrors.GroupHasStudents);

        // Quitar asignaciones de docentes antes de eliminar el grupo
        var teacherAssignments = await _context.UserGroup.Where(ug => ug.GroupId == groupId).ToListAsync();
        if (teacherAssignments.Count > 0)
            _context.UserGroup.RemoveRange(teacherAssignments);

        _context.Group.Remove(group);
        await _context.SaveChangesAsync();

        return Result<int>.Success(groupId);
    }

    public async Task<IEnumerable<EducationLevel>> GetEducationLevels()
    {
        return await _context.EducationLevel
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Orden)
            .ToListAsync();
    }

    public async Task<IEnumerable<Grade>> GetGrades(int? educationLevelId = null)
    {
        var query = _context.Grade.AsNoTracking().Include(g => g.EducationLevel).AsQueryable();

        if (educationLevelId.HasValue)
            query = query.Where(g => g.EducationLevelId == educationLevelId.Value);

        return await query
            .OrderBy(g => g.EducationLevel.Orden)
            .ThenBy(g => g.Numero)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetRoles()
    {
        return await _context.Role
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .ToListAsync();
    }
}
