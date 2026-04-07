namespace Models.DB;

public class CIEDimension
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ColorHex { get; set; }
    public string? Description { get; set; }
    public short Order { get; set; }
}
