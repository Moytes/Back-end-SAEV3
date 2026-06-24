using Models.DB;

namespace Models.Dto;

public class AttentionModeItemDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public int SchoolYearId { get; set; }
    public Phases Phase { get; set; }
    public AttentionTypes Type { get; set; }
}
