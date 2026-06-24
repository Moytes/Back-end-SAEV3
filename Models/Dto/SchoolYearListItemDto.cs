namespace Models.Dto;

public class SchoolYearListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool Activo { get; set; }
}
