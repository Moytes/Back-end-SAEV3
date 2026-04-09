using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddMaterialRequest
{
    [Required]
    public Guid CreatorId { get; set; }

    [Required]
    public Guid MaterialTypeId { get; set; }

    public Guid? DimensionId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Range(1, 6)]
    public short? GradeMin { get; set; }

    [Range(1, 6)]
    public short? GradeMax { get; set; }

    public string? ContentJson { get; set; }
    public string? FileUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public boolStatus AutoEvaluation { get; set; } = boolStatus.False;
    public string? CriteriaJson { get; set; }
    public boolStatus Published { get; set; } = boolStatus.False;
}