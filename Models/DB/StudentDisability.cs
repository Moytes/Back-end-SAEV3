namespace Models.DB;

public class StudentDisability
{
    public Guid Id { get; set; }
    public boolStatus ExternalDiagnosis { get; set; } = boolStatus.False;
    public string? FileUrl { get; set; }
    public string? Notes { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // Disability
    public Disability Disability { get; set; } = null!;
    public Guid DisabilityId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
