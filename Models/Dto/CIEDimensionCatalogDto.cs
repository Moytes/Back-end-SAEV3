namespace Models.Dto;

public class CIEDimensionCatalogDto
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ColorHex { get; set; }
    public string? Description { get; set; }
    public short Order { get; set; }
    public List<CIEIndicatorCatalogDto> Indicators { get; set; } = [];
}

public class CIEIndicatorCatalogDto
{
    public Guid Id { get; set; }
    public Guid DimensionId { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public short Order { get; set; }
    public List<CIESubIndicatorCatalogDto> SubIndicators { get; set; } = [];
}

public class CIESubIndicatorCatalogDto
{
    public Guid Id { get; set; }
    public Guid IndicatorId { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public short Order { get; set; }
}