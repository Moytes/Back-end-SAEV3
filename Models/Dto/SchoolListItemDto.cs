namespace Models.Dto;

public class SchoolListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CCT { get; set; }
    public int Turn { get; set; }
    public string TurnName { get; set; } = null!;
    public string? Address { get; set; }
    public bool IsActive { get; set; }

    public Guid SchoolZoneId { get; set; }
    public string SchoolZoneNumber { get; set; } = null!;
    public string? SchoolZoneName { get; set; }
}