namespace Models.DB;

public class SchoolZone
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public string CCT { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }

    public int? AcademySubscriptionId { get; set; }
    public AcademySubscription? AcademySubscription { get; set; }

    public ICollection<School> Schools { get; set; } = new List<School>();
}
