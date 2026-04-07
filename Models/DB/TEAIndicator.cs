namespace Models.DB;

public enum teaDomain
{
    COMUNICACION_SOCIAL = 0,
    CONDUCTA_REPETITIVA = 1
}

public class TEAIndicator
{
    public Guid Id { get; set; }
    public teaDomain Domain { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public short? AgeRangeMin { get; set; } // months
    public short? AgeRangeMax { get; set; } // months
    public short Order { get; set; }
}
