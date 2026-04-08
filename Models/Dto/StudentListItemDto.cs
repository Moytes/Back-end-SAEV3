using Models.DB;

namespace Models.Dto;

public class StudentListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public gender Gender { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }
    public string? PhotoUrl { get; set; }
    public boolStatus Status { get; set; }

    public Guid? SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public Guid? SchoolYearId { get; set; }
    public string? SchoolYearName { get; set; }

    public string FullName => string.Join(" ", new[]
    {
        Name,
        FatherLastName,
        MotherLastName
    }.Where(x => !string.IsNullOrWhiteSpace(x)));
}