namespace Models.Dto;

public class StudentAttentionAreaItemDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public int AttentionAreaId { get; set; }
    public string AttentionAreaCVE { get; set; } = null!;
    public string AttentionAreaName { get; set; } = null!;
    public int SchoolYearId { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}
