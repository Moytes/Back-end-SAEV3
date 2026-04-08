using System.Data;
using Dapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Models.Dto;
using Repositories.IRepositories;

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
        var sql = """
            SELECT
                s.id,
                s.name,
                s.cct,
                CAST(s.turn AS integer) AS Turn,
                CASE CAST(s.turn AS integer)
                    WHEN 1 THEN 'MATUTINO'
                    WHEN 2 THEN 'VESPERTINO'
                    WHEN 3 THEN 'COMPLETO'
                    ELSE 'DESCONOCIDO'
                END AS TurnName,
                s.address,
                CASE WHEN CAST(s.status AS integer) = 1 THEN TRUE ELSE FALSE END AS IsActive,
                s.school_zone_id AS SchoolZoneId,
                sz.number AS SchoolZoneNumber,
                sz.name AS SchoolZoneName
            FROM school s
            INNER JOIN school_zone sz ON sz.id = s.school_zone_id
            WHERE (@SchoolZoneId IS NULL OR s.school_zone_id = @SchoolZoneId)
            ORDER BY sz.number, s.name;
            """;

        return await _dbConnection.QueryAsync<SchoolListItemDto>(sql, new
        {
            SchoolZoneId = schoolZoneId
        });
    }

    public async Task<IEnumerable<GroupListItemDto>> GetGroups(Guid? schoolId = null, Guid? schoolYearId = null)
    {
        var sql = """
            SELECT
                g.id,
                g.school_id AS SchoolId,
                s.name AS SchoolName,
                s.cct AS SchoolCCT,
                g.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                CAST(g.grade AS integer) AS Grade,
                CASE CAST(g.grade AS integer)
                    WHEN 1 THEN 'Primero'
                    WHEN 2 THEN 'Segundo'
                    WHEN 3 THEN 'Tercero'
                    WHEN 4 THEN 'Cuarto'
                    WHEN 5 THEN 'Quinto'
                    WHEN 6 THEN 'Sexto'
                    ELSE 'Sin grado'
                END AS GradeName,
                g.section AS Section,
                CONCAT(
                    CASE CAST(g.grade AS integer)
                        WHEN 1 THEN '1°'
                        WHEN 2 THEN '2°'
                        WHEN 3 THEN '3°'
                        WHEN 4 THEN '4°'
                        WHEN 5 THEN '5°'
                        WHEN 6 THEN '6°'
                        ELSE '?°'
                    END,
                    ' ',
                    g.section
                ) AS DisplayName
            FROM "group" g
            INNER JOIN "school" s ON s.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = g.school_year_id
            WHERE (@SchoolId IS NULL OR g.school_id = @SchoolId)
              AND (@SchoolYearId IS NULL OR g.school_year_id = @SchoolYearId)
            ORDER BY sy.name DESC, s.name, Grade, Section;
            """;

        return await _dbConnection.QueryAsync<GroupListItemDto>(sql, new
        {
            SchoolId = schoolId,
            SchoolYearId = schoolYearId
        });
    }
}