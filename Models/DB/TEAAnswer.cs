namespace Models.DB;

public class TEAAnswer
{
    public Guid Id { get; set; }
    public short? Frequency { get; set; } // 0-3
    public short? Intensity { get; set; } // 0-3
    public string? Observation { get; set; }

    // TEA Screening
    public TEAScreening Screening { get; set; } = null!;
    public Guid ScreeningId { get; set; }

    // TEA Indicator
    public TEAIndicator Indicator { get; set; } = null!;
    public Guid IndicatorId { get; set; }
}
