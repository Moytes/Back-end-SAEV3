namespace Models.DB;

public enum assignmentStudentStatus
{
    PENDIENTE = 0,
    EN_PROGRESO = 1,
    COMPLETADO = 2,
    EVALUADO = 3
}

public class AssignmentStudent
{
    public Guid Id { get; set; }
    public assignmentStudentStatus Status { get; set; } = assignmentStudentStatus.PENDIENTE;
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? ResponseJson { get; set; } // JSONB
    public string[]? EvidenceUrls { get; set; }
    public string? AutoEvaluationJson { get; set; } // JSONB
    public string? ManualGradeJson { get; set; } // JSONB
    public DateTime? EvaluationDate { get; set; }
    public string? Feedback { get; set; }

    // Assignment
    public Assignment Assignment { get; set; } = null!;
    public Guid AssignmentId { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // Evaluated by (optional)
    public User? EvaluatedBy { get; set; }
    public Guid? EvaluatedById { get; set; }
}
