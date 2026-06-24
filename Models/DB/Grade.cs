namespace Models.DB;

public class Grade
{
    public int Id { get; set; }
    public short Numero { get; set; }
    public string Nombre { get; set; } = null!;

    public EducationLevel EducationLevel { get; set; } = null!;
    public int EducationLevelId { get; set; }
}
