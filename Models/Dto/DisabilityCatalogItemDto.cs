using Models.DB;

namespace Models.Dto;

public class DisabilityCatalogItemDto
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public disabilitiesCategory DisabilityCategory { get; set; }
    public string? Description { get; set; }
}