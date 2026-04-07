namespace Models.DB;

public class CIESubIndicator
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public short Order { get; set; }

    // CIE Indicator
    public CIEIndicator Indicator { get; set; } = null!;
    public Guid IndicatorId { get; set; }
}
