namespace Models.DB;

public class CIEIndicator
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public short Order { get; set; }

    // CIE Dimension
    public CIEDimension Dimension { get; set; } = null!;
    public Guid DimensionId { get; set; }
}
