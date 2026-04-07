namespace Models.DB;

public enum gender
{
    M = 1, // Male (Masculino)
    H = 2  // Female (Hombre is incorrect in DB, should be F but keeping H for consistency)
}

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public gender Gender { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }
    public string? PhotoUrl { get; set; }
    public boolStatus Status { get; set; } = boolStatus.True;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
