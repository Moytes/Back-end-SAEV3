namespace Models.DB;

public class DocenteObservacion
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Guid DocenteId { get; set; }
    public User Docente { get; set; } = null!;
    public int? SchoolYearId { get; set; }
    public SchoolYear? SchoolYear { get; set; }
    public string Texto { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
