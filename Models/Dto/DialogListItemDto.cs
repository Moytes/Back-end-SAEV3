namespace Models.Dto;

public class DialogListItemDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialTitle { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? EstimatedDurationMin { get; set; }
    public DateTime CreatedAt { get; set; }
}