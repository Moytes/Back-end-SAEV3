using System.Net;

namespace Models.DB;

public class AuditLog
{
    public long Id { get; set; }
    public string Action { get; set; } = null!;
    public string? AffectedTable { get; set; }
    public string? RecordId { get; set; }
    public string? Request { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User (optional, for system actions)
    public User? User { get; set; }
    public Guid? UserId { get; set; }
}
