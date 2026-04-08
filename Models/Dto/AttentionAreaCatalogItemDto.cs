namespace Models.Dto;

public class AttentionAreaCatalogItemDto
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
}