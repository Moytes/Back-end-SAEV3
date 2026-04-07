namespace Models.DB;

public class SchoolYear
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public boolStatus Status { get; set; } = boolStatus.False;
}
