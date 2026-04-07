namespace Models.DB;

public enum Turns
{
    MATUTINO = 1,
    VESPERTINO = 2,
    COMPLETO = 3
}

public class School
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CCT { get; set; }
    public Turns Turn { get; set; } = Turns.MATUTINO;
    public string? Address { get; set; }
    public boolStatus Status { get; set; } = boolStatus.True;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relation with school zone
    public SchoolZone SchoolZone { get; set; } = null!;
    public Guid SchoolZoneId { get; set; }
}