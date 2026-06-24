namespace Models.DB;

public class Role
{
    public int Id { get; set; }
    public string Clave { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Permisos { get; set; }
}
