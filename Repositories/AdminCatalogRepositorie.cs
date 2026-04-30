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
        {
            var status = onlyActive.Value ? boolStatus.True : boolStatus.False;
            query = query.Where(x => x.Status == status);
        }

        return await query
            .OrderByDescending(x => x.Status)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new SchoolYearListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.Status == boolStatus.True
            })
            .ToListAsync();
    }

    public async Task<Result<Guid>> CreateSchoolYear(AddSchoolYearRequest request)
    {
        var nameExists = await _context.SchoolYear.AnyAsync(x => x.Name == request.Name);
        if (nameExists)
            return Result<Guid>.Failure(new Error("SchoolYear.DuplicateName", "Ya existe un ciclo escolar con ese nombre."));

        if (request.IsActive)
        {
            var activeYears = await _context.SchoolYear.Where(x => x.Status == boolStatus.True).ToListAsync();
            foreach (var year in activeYears)
            {
                year.Status = boolStatus.False;
            }
        }

        var schoolYear = new SchoolYear
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.IsActive ? boolStatus.True : boolStatus.False
        };

        await _context.SchoolYear.AddAsync(schoolYear);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(schoolYear.Id);
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

    public async Task<IEnumerable<SchoolListItemDto>> GetSchools(Guid? schoolZoneId = null)
    {
        return await _context.School
            .AsNoTracking()
            .Include(s => s.SchoolZone)
            .Where(s => !schoolZoneId.HasValue || s.SchoolZoneId == schoolZoneId)
            .OrderBy(s => s.SchoolZone.Number)
            .ThenBy(s => s.Name)
            .Select(s => new SchoolListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                CCT = s.CCT,
                Turn = (int)s.Turn,
                TurnName = s.Turn.ToString().ToUpper(),
                Address = s.Address,
                IsActive = s.Status == boolStatus.True,
                SchoolZoneId = s.SchoolZoneId,
                SchoolZoneNumber = s.SchoolZone.Number,
                SchoolZoneName = s.SchoolZone.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupListItemDto>> GetGroups(Guid? schoolId = null, Guid? schoolYearId = null)
    {
        return await _context.Group
            .AsNoTracking()
            .Include(g => g.School)
            .Include(g => g.SchoolYear)
            .Where(g => (!schoolId.HasValue || g.SchoolId == schoolId) &&
                        (!schoolYearId.HasValue || g.SchoolYearId == schoolYearId))
            .OrderByDescending(g => g.SchoolYear.Name)
            .ThenBy(g => g.School.Name)
            .ThenBy(g => g.Grade)
            .ThenBy(g => g.Section)
            .Select(g => new GroupListItemDto
            {
                Id = g.Id,
                SchoolId = g.SchoolId,
                SchoolName = g.School.Name,
                SchoolCCT = g.School.CCT,
                SchoolYearId = g.SchoolYearId,
                SchoolYearName = g.SchoolYear.Name,
                Grade = (int)g.Grade,
                GradeName = g.Grade.ToString(), // O mapear según lógica previa
                Section = g.Section,
                DisplayName = $"{ (int)g.Grade }° { g.Section }"
            })
            .ToListAsync();
    }

    public async Task<Result<Guid>> CreateSchoolZone(AddSchoolZoneRequest request)
    {
        var cctExists = await _context.SchoolZone.AnyAsync(x => x.CCT == request.CCT);
        if (cctExists)
            return Result<Guid>.Failure(SchoolErrors.CctAlreadyExists);

        var zone = new SchoolZone
        {
            Id = Guid.NewGuid(),
            Number = request.Number,
            CCT = request.CCT,
            Name = request.Name,
            Description = request.Description
        };

        await _context.SchoolZone.AddAsync(zone);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(zone.Id);
    }

    public async Task<Result<Guid>> CreateSchool(AddSchoolRequest request)
    {
        var zoneExists = await _context.SchoolZone.AnyAsync(x => x.Id == request.SchoolZoneId);
        if (!zoneExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolZoneNotFound);

        var cctExists = await _context.School.AnyAsync(x => x.CCT == request.CCT);
        if (cctExists)
            return Result<Guid>.Failure(SchoolErrors.CctAlreadyExists);

        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CCT = request.CCT,
            Turn = request.Turn,
            Address = request.Address,
            SchoolZoneId = request.SchoolZoneId,
            Status = boolStatus.True,
            CreatedAt = DateTime.UtcNow
        };

        await _context.School.AddAsync(school);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(school.Id);
    }

    public async Task<Result<Guid>> CreateGroup(AddGroupRequest request)
    {
        var schoolExists = await _context.School.AnyAsync(x => x.Id == request.SchoolId);
        if (!schoolExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolNotFound);

        var yearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!yearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        var groupExists = await _context.Group.AnyAsync(x =>
            x.SchoolId == request.SchoolId &&
            x.Grade == request.Grade &&
            x.Section == request.Section &&
            x.SchoolYearId == request.SchoolYearId);

        if (groupExists)
            return Result<Guid>.Failure(GroupErrors.GroupAlreadyExists);

        var group = new Group
        {
            Id = Guid.NewGuid(),
            SchoolId = request.SchoolId,
            Grade = request.Grade,
            Section = request.Section,
            SchoolYearId = request.SchoolYearId
        };

        await _context.Group.AddAsync(group);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(group.Id);
    }
}
