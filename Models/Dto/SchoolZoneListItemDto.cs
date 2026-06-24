namespace Models.Dto;

public class SchoolZoneListItemDto
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public string CCT { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
}
