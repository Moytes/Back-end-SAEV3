using Models.DB;

namespace Models.Dto;

public class StudentListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public Sexo Sexo { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? CURP { get; set; }
    public string? PhotoUrl { get; set; }
    public bool Activo { get; set; }

    public int? SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int? GradeId { get; set; }
    public int? EducationLevelId { get; set; }
    public int? SchoolYearId { get; set; }
    public string? SchoolYearName { get; set; }

    public string FullName => string.Join(" ", new[]
    {
        Name,
        FatherLastName,
        MotherLastName
    }.Where(x => !string.IsNullOrWhiteSpace(x)));
}
