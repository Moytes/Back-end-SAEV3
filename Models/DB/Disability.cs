namespace Models.DB;

public enum disabilitiesCategory
{
    DISCAPACIDAD = 1,
    BAP = 2,
    AS = 3
}

public class Disability
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public disabilitiesCategory DisabilityCategory { get; set; }
    public string? Description { get; set; }
}
