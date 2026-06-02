namespace Models.DB;

public enum Gender
{
    M = 1, // Male (Masculino)
    F = 2  // Female (Femenino)
}

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }
    public string? PhotoUrl { get; set; }
    public BoolStatus Status { get; set; } = BoolStatus.True;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
