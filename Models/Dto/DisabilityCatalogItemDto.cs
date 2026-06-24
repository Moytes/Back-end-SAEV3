using Models.DB;

namespace Models.Dto;

public class DisabilityCatalogItemDto
{
    public int Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DisabilityCategory Category { get; set; }
    public string? Description { get; set; }
}
