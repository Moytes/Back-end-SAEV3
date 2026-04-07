namespace Models.DB;

public class DialogProgress
{
    public Guid Id { get; set; }
    public short CurrentScene { get; set; } = 0;
    public string? ResponsesJson { get; set; } // JSONB
    public decimal? Score { get; set; }
    public boolStatus Completed { get; set; } = boolStatus.False;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }

    // Dialog
    public Dialog Dialog { get; set; } = null!;
    public Guid DialogId { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }
}
