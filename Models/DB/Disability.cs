namespace Models.DB;

public class Disability
{
    public int Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DisabilityCategory Category { get; set; }
    public string? Description { get; set; }
}
