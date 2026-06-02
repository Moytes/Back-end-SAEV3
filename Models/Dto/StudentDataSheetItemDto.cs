using Models.DB;

namespace Models.Dto;

public class StudentDataSheetItemDto
{
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Gender Gender { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }

    public Guid? SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public Guid? SchoolYearId { get; set; }
    public string? SchoolYearName { get; set; }

    public string[] Disabilities { get; set; } = [];
    public string[] AttentionAreas { get; set; } = [];
    public string? AttentionMode { get; set; }
}