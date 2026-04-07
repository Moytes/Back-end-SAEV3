namespace Models.DB;

public class Material
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? GradeMin { get; set; }
    public short? GradeMax { get; set; }
    public string? ContentJson { get; set; } // JSONB
    public string? FileUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public boolStatus AutoEvaluation { get; set; } = boolStatus.False;
    public string? CriteriaJson { get; set; } // JSONB
    public boolStatus Published { get; set; } = boolStatus.False;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Creator
    public User Creator { get; set; } = null!;
    public Guid CreatorId { get; set; }

    // Material Type
    public MaterialType MaterialType { get; set; } = null!;
    public Guid MaterialTypeId { get; set; }

    // CIE Dimension (optional)
    public CIEDimension? Dimension { get; set; }
    public Guid? DimensionId { get; set; }
}
