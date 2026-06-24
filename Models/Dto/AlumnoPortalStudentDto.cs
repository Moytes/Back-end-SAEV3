namespace Models.Dto;

public class AlumnoPortalStudentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public int SchoolId { get; set; }
    public string SchoolName { get; set; } = null!;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public int EducationLevelId { get; set; }
    public string EducationLevelName { get; set; } = null!;
    public bool AccessedByTutor { get; set; }
}
