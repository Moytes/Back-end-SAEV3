namespace Models.DB;

public class PsychoCollaborator
{
    public Guid Id { get; set; }
    public string? ExternalName { get; set; }
    public string? CollaboratorRole { get; set; }
    public boolStatus DigitalSignature { get; set; } = boolStatus.False;
    public DateTime? SignatureDate { get; set; }

    // Psychoeducational Assessment
    public PsychoeducationalAssessment PsychoeducationalAssessment { get; set; } = null!;
    public Guid PsychoeducationalAssessmentId { get; set; }

    // User (optional, for internal collaborators)
    public User? User { get; set; }
    public Guid? UserId { get; set; }
}
