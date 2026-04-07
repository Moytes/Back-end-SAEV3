namespace Models.DB;

public class CIEPhonologyAnswer
{
    public Guid Id { get; set; }
    public bool? Functional { get; set; }
    public string? ObservationForm { get; set; }

    // CIE Evaluation
    public CIEEvaluation Evaluation { get; set; } = null!;
    public Guid EvaluationId { get; set; }

    // CIE SubIndicator
    public CIESubIndicator SubIndicator { get; set; } = null!;
    public Guid SubIndicatorId { get; set; }
}
