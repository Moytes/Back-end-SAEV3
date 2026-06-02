using Models.DB;

namespace Models.Dto;

public class StudentAttentionAreaItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid AttentionAreaId { get; set; }
    public string AttentionAreaCVE { get; set; } = null!;
    public string AttentionAreaName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public BoolStatus IsRequired { get; set; }
    public string? Notes { get; set; }
}