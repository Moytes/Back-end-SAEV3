namespace Models.Dto;

public class MaterialTypeCatalogItemDto
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}