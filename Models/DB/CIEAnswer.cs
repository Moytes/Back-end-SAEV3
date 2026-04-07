namespace Models.DB;

public enum answerType
{
    COMUNICATIVO = 0,
    LINGUISTICO = 1
}

public class CIEAnswer
{
    public Guid Id { get; set; }
    public bool? Achieved { get; set; }
    public short? HelpLevel { get; set; } // 0=Independiente, 1=Verbal, 2=Visual, 3=Física, 4=Total
    public answerType? ResponseType { get; set; }
    public string? Observation { get; set; }
    public string? EvidenceUrl { get; set; }

    // CIE Evaluation
    public CIEEvaluation Evaluation { get; set; } = null!;
    public Guid EvaluationId { get; set; }

    // CIE SubIndicator
    public CIESubIndicator SubIndicator { get; set; } = null!;
    public Guid SubIndicatorId { get; set; }
}
