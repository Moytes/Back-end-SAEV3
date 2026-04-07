namespace Models.DB;

public class PsychoBAP
{
    public Guid Id { get; set; }
    public string? BAPType { get; set; }
    public string? Context { get; set; }
    public string? InclusionIndicator { get; set; }
    public string? Description { get; set; }

    // Psychoeducational Assessment
    public PsychoeducationalAssessment PsychoeducationalAssessment { get; set; } = null!;
    public Guid PsychoeducationalAssessmentId { get; set; }
}
