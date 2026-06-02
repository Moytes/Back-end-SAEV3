using Models.DB;

namespace Models.Request;

public class PsychoCollaboratorItemRequest
{
    public Guid? UserId { get; set; }
    public string? ExternalName { get; set; }
    public string? CollaboratorRole { get; set; }
    public BoolStatus DigitalSignature { get; set; } = BoolStatus.False;
    public DateTime? SignatureDate { get; set; }
}