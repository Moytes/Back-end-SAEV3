namespace Models.Dto;

public class GroupListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolName { get; set; } = null!;
    public string? SchoolCCT { get; set; }
    public int SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public int GradeId { get; set; }
    public string GradeName { get; set; } = null!;
    public int GradeNumber { get; set; }
    public int EducationLevelId { get; set; }
    public string EducationLevelName { get; set; } = null!;
    public string Section { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
