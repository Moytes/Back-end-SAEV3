using Models.DB;

namespace Models.Dto;

public class MaterialListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? GradeMin { get; set; }
    public short? GradeMax { get; set; }
    public string? FileUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public boolStatus AutoEvaluation { get; set; }
    public boolStatus Published { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid CreatorId { get; set; }
    public string CreatorFullName { get; set; } = null!;

    public Guid MaterialTypeId { get; set; }
    public string MaterialTypeCVE { get; set; } = null!;
    public string MaterialTypeName { get; set; } = null!;

    public Guid? DimensionId { get; set; }
    public string? DimensionCVE { get; set; }
    public string? DimensionName { get; set; }

    public string[] Tags { get; set; } = [];
}