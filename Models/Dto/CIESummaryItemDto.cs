namespace Models.Dto;

public class CIESummaryItemDto
{
    public Guid EvaluationId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid DimensionId { get; set; }
    public string DimensionName { get; set; } = null!;
    public string DimensionCVE { get; set; } = null!;
    public int TotalAnswers { get; set; }
    public int AchievedAnswers { get; set; }
    public decimal AchievedPercentage { get; set; }
}