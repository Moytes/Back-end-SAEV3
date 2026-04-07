namespace Models.DB;

public enum canalizationStatus
{
    PENDIENTE = 0,
    RECIBIDA = 1,
    EN_PROCESO = 2,
    CERRADA = 3
}

public class Canalization
{
    public Guid Id { get; set; }
    public DateOnly CanalizationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string Reason { get; set; } = null!;
    public string ClassroomActions { get; set; } = null!;
    public DateOnly? ReceivedDate { get; set; }
    public canalizationStatus Status { get; set; } = canalizationStatus.PENDIENTE;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }
    
    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }

    // Attention area
    public AttentionArea AttentionArea { get; set; } = null!;
    public Guid AttentionAreaId { get; set; }

    // Requester user
    public User Requester { get; set; } = null!;
    public Guid RequesterId { get; set; }

    // Receiver user
    public User Receiver { get; set; } = null!;
    public Guid ReceiverId { get; set; }
}
