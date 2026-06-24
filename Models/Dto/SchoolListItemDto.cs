namespace Models.Dto;

public class SchoolListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CCT { get; set; }
    public int Turn { get; set; }
    public string TurnName { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool Activa { get; set; }
    public int EducationLevelId { get; set; }
    public string EducationLevelName { get; set; } = null!;
    public int? SchoolZoneId { get; set; }
    public string? SchoolZoneNumber { get; set; }
    public string? SchoolZoneName { get; set; }
}
