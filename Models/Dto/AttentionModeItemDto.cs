using Models.DB;

namespace Models.Dto;

public class AttentionModeItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid SchoolYearId { get; set; }
    public phases Phase { get; set; }
    public attentionTypes Type { get; set; }
}
