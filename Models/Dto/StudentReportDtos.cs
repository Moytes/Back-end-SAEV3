namespace Models.Dto;

public class ActividadReporteDto
{
    public string Titulo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaLimite { get; set; }
}

public class EvaluacionResumenDto
{
    public string Tipo { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string? Estado { get; set; }
}
