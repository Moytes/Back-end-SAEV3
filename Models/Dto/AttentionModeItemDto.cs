using Models.DB;

namespace Models.Dto;

public class AttentionModeItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid SchoolYearId { get; set; }
    public Phases Phase { get; set; }
    public AttentionTypes Type { get; set; }
}
