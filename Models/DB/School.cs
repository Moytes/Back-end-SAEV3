namespace Models.DB;

public class School
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CCT { get; set; }
    public Turns Turn { get; set; } = Turns.MATUTINO;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? SchoolZoneId { get; set; }
    public SchoolZone? SchoolZone { get; set; }

    public int EducationLevelId { get; set; }
    public EducationLevel EducationLevel { get; set; } = null!;

    public int? AcademySubscriptionId { get; set; }
    public AcademySubscription? AcademySubscription { get; set; }
}
