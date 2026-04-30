namespace Models.Dto;

public class GroupListItemDto
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }
    public string SchoolName { get; set; } = null!;
    public string? SchoolCCT { get; set; }

    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;

    public int Grade { get; set; }
    public string GradeName { get; set; } = null!;

    public string Section { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}