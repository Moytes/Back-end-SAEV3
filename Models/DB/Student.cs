namespace Models.DB;

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public Sexo Sexo { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }
    public string? PhotoUrl { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? SchoolId { get; set; }
    public School? School { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
