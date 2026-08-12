namespace Models.Dto;

public class ActividadReporteDto
{
    public string Titulo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaLimite { get; set; }
}

public class StudentHistorialActividadDto
{
    public int Id { get; set; }
    public string MaterialTitulo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaLimite { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public string? Retroalimentacion { get; set; }
    public string? Instrucciones { get; set; }
}

public class EvaluacionResumenDto
{
    public string Tipo { get; set; } = null!;
    // DateOnly y no DateTime: la columna de origen es `date` en Postgres, y Npgsql la
    // entrega como DateOnly nativo — declararla DateTime causaba "Object must implement
    // IConvertible" al deserializar (Dapper no convierte DateOnly->DateTime implícitamente).
    public DateOnly Fecha { get; set; }
    public string? Estado { get; set; }
}

public class DocenteObservacionDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid DocenteId { get; set; }
    public string? DocenteNombre { get; set; }
    public int? SchoolYearId { get; set; }
    public string Texto { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
