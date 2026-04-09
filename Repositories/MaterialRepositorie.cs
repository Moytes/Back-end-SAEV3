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

public class MaterialRepositorie : IMaterialRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public MaterialRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<MaterialTypeCatalogItemDto>> GetMaterialTypes()
    {
        return await _context.MaterialType
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new MaterialTypeCatalogItemDto
            {
                Id = x.Id,
                CVE = x.CVE,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<MaterialListItemDto>> GetMaterials(string? tag, Guid? dimensionId, short? grade)
    {
        var sql = """
            SELECT
                m.id,
                m.title,
                m.description,
                m.grade_min AS GradeMin,
                m.grade_max AS GradeMax,
                m.file_url AS FileUrl,
                m.thumbnail_url AS ThumbnailUrl,
                m.auto_evaluation AS AutoEvaluation,
                m.published AS Published,
                m.created_at AS CreatedAt,

                m.creator_id AS CreatorId,
                CONCAT(u.name, ' ', u.father_last_name, ' ', COALESCE(u.mother_last_name, '')) AS CreatorFullName,

                m.material_type_id AS MaterialTypeId,
                mt.cve AS MaterialTypeCVE,
                mt.name AS MaterialTypeName,

                m.dimension_id AS DimensionId,
                d.cve AS DimensionCVE,
                d.name AS DimensionName,

                COALESCE(
                    ARRAY_REMOVE(ARRAY_AGG(DISTINCT tg.tag), NULL),
                    ARRAY[]::text[]
                ) AS Tags
            FROM "material" m
            INNER JOIN "user" u ON u.id = m.creator_id
            INNER JOIN "material_type" mt ON mt.id = m.material_type_id
            LEFT JOIN "cie_dimension" d ON d.id = m.dimension_id
            LEFT JOIN "material_tag" tg ON tg.material_id = m.id
            WHERE (@Tag IS NULL OR EXISTS (
                    SELECT 1
                    FROM "material_tag" mtg
                    WHERE mtg.material_id = m.id
                      AND mtg.tag ILIKE @TagPattern
                ))
              AND (@DimensionId IS NULL OR m.dimension_id = @DimensionId)
              AND (@Grade IS NULL OR (
                    (@Grade >= COALESCE(m.grade_min, @Grade))
                    AND (@Grade <= COALESCE(m.grade_max, @Grade))
                ))
            GROUP BY
                m.id, u.id, mt.id, d.id
            ORDER BY m.created_at DESC, m.title;
            """;

        return await _dbConnection.QueryAsync<MaterialListItemDto>(sql, new
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? null : tag.Trim(),
            TagPattern = $"%{tag?.Trim()}%",
            DimensionId = dimensionId,
            Grade = grade
        });
    }

    public async Task<Result<Guid>> CreateMaterial(AddMaterialRequest request)
    {
        var creatorExists = await _context.User.AnyAsync(x => x.Id == request.CreatorId);
        if (!creatorExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var materialTypeExists = await _context.MaterialType.AnyAsync(x => x.Id == request.MaterialTypeId);
        if (!materialTypeExists)
            return Result<Guid>.Failure(MaterialErrors.MaterialTypeNotFound);

        if (request.DimensionId.HasValue)
        {
            var dimensionExists = await _context.CIEDimension.AnyAsync(x => x.Id == request.DimensionId.Value);
            if (!dimensionExists)
                return Result<Guid>.Failure(CIEErrors.DimensionNotFound);
        }

        if (request.GradeMin.HasValue && request.GradeMax.HasValue && request.GradeMin > request.GradeMax)
            return Result<Guid>.Failure(MaterialErrors.InvalidGradeRange);

        var entity = new Material
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            MaterialTypeId = request.MaterialTypeId,
            DimensionId = request.DimensionId,
            Title = request.Title,
            Description = request.Description,
            GradeMin = request.GradeMin,
            GradeMax = request.GradeMax,
            ContentJson = request.ContentJson,
            FileUrl = request.FileUrl,
            ThumbnailUrl = request.ThumbnailUrl,
            AutoEvaluation = request.AutoEvaluation,
            CriteriaJson = request.CriteriaJson,
            Published = request.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Material.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<List<Guid>>> AssignTags(Guid materialId, AssignMaterialTagsRequest request)
    {
        var material = await _context.Material.FirstOrDefaultAsync(x => x.Id == materialId);
        if (material == null)
            return Result<List<Guid>>.Failure(MaterialErrors.MaterialNotFound);

        var normalizedTags = request.Tags
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedTags.Count == 0)
            return Result<List<Guid>>.Failure(MaterialErrors.EmptyTagsAreNotAllowed);

        await using var tx = await _context.Database.BeginTransactionAsync();

        var currentRows = await _context.MaterialTag
            .Where(x => x.MaterialId == materialId)
            .ToListAsync();

        _context.MaterialTag.RemoveRange(currentRows);

        var newRows = normalizedTags.Select(tag => new MaterialTag
        {
            Id = Guid.NewGuid(),
            MaterialId = materialId,
            Tag = tag
        }).ToList();

        await _context.MaterialTag.AddRangeAsync(newRows);

        material.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<List<Guid>>.Success(newRows.Select(x => x.Id).ToList());
    }

    public async Task<Material?> GetMaterialById(Guid materialId)
    {
        return await _context.Material.FirstOrDefaultAsync(x => x.Id == materialId);
    }

    public async Task<IEnumerable<DialogListItemDto>> GetDialogs(Guid? materialId = null)
    {
        var sql = """
            SELECT
                d.id,
                d.material_id AS MaterialId,
                m.title AS MaterialTitle,
                d.title,
                d.description,
                d.estimated_duration_min AS EstimatedDurationMin,
                d.created_at AS CreatedAt
            FROM "dialog" d
            INNER JOIN "material" m ON m.id = d.material_id
            WHERE (@MaterialId IS NULL OR d.material_id = @MaterialId)
            ORDER BY d.created_at DESC, d.title;
            """;
        return await _dbConnection.QueryAsync<DialogListItemDto>(sql, new
        {
            MaterialId = materialId
        });
    }

    public async Task<Result<Guid>> CreateDialog(AddDialogRequest request)
    {
        var material = await _context.Material
            .Include(x => x.MaterialType)
            .FirstOrDefaultAsync(x => x.Id == request.MaterialId);

        if (material == null)
            return Result<Guid>.Failure(MaterialErrors.MaterialNotFound);

        if (!string.Equals(material.MaterialType.CVE, "DIALOGO_ANIMADO", StringComparison.OrdinalIgnoreCase))
            return Result<Guid>.Failure(MaterialErrors.MaterialMustBeDialogType);

        var entity = new Dialog
        {
            Id = Guid.NewGuid(),
            MaterialId = request.MaterialId,
            Title = request.Title,
            Description = request.Description,
            CharacterJson = request.CharacterJson,
            ScenesJson = request.ScenesJson,
            EstimatedDurationMin = request.EstimatedDurationMin,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Dialog.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }
}