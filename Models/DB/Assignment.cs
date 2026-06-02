namespace Models.DB;

public class Assignment
{
    public Guid Id { get; set; }
    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    public DateOnly? DueDate { get; set; }
    public string? Instructions { get; set; }
    public BoolStatus Active { get; set; } = BoolStatus.True;

    // Material
    public Material Material { get; set; } = null!;
    public Guid MaterialId { get; set; }

    // Assigned by
    public User AssignedBy { get; set; } = null!;
    public Guid AssignedById { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
