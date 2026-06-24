namespace Models.DB;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Clave { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal PrecioMensual { get; set; }
    public decimal? PrecioAnual { get; set; }
    public int? MaxUsuarios { get; set; }
    public int? MaxAlumnos { get; set; }
    public string? Caracteristicas { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
