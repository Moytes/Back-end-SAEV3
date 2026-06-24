namespace Models.DB;

public class EducationLevel
{
    public int Id { get; set; }
    public string Clave { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public short Orden { get; set; }
    public bool Activo { get; set; } = true;
}
