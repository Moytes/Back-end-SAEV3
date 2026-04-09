using Models.DB;

namespace Models.Dto;

public class TEAIndicatorCatalogItemDto
{
    public Guid Id { get; set; }
    public teaDomain Domain { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public short? AgeRangeMin { get; set; }
    public short? AgeRangeMax { get; set; }
    public short Order { get; set; }
}