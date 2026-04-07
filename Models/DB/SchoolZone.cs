namespace Models.DB;

public class SchoolZone
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public string CCT { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Relation with schools
    public ICollection<School> Schools { get; set; } = new List<School>();
}
