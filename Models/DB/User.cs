using System.ComponentModel.DataAnnotations;

namespace Models.DB;

public enum UserRole
{
    [Display(Name = "Administrador del sistema")]
    ADMIN = 1,

    [Display(Name = "Supervisor de zona")]
    SUPERVISOR = 2,

    [Display(Name = "Director de USAER")]
    DIRECTOR_USAER = 3,

    [Display(Name = "Especialista en Comunicación")]
    ESPECIALISTA_COM = 4,

    [Display(Name = "Especialista en Psicología")]
    ESPECIALISTA_PSI = 5,

    [Display(Name = "Especialista en Aprendizaje")]
    ESPECIALISTA_APR = 6,

    [Display(Name = "Trabajo Social")]
    TRABAJO_SOCIAL = 7,

    [Display(Name = "Docente de grupo regular")]
    DOCENTE = 8,

    [Display(Name = "Alumno")]
    STUDENT = 9
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public UserRole Role { get; set; }
    public string? PhoneNumber { get; set; }
    public boolStatus Status { get; set; } = boolStatus.True;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    // Relation with school zone (optional)
    public SchoolZone? SchoolZone { get; set; }
    public Guid? SchoolZoneId { get; set; }

    // Relation with student (optional, used for student login)
    public Student? Student { get; set; }
    public Guid? StudentId { get; set; }
}
