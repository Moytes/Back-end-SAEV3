using Models.DB;

namespace Models.Dto;

public class StudentRecordDto
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<TutorListItemDto> Tutors { get; set; } = [];
    public List<StudentRegistrationItemDto> Registrations { get; set; } = [];
}
