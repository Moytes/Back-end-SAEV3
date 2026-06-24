namespace Models.DB;

public class StudentDisability
{
    public int Id { get; set; }
    public bool ExternalDiagnosis { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Notes { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int DisabilityId { get; set; }
    public Disability Disability { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
