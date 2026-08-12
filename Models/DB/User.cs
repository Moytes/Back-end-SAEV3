namespace Models.DB;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public string? Phone { get; set; }
    public bool Activo { get; set; } = true;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Especialidad del especialista (PSICOLOGIA/COMUNICACION/APRENDIZAJE) — solo determina
    /// su pantalla de inicio y filtro sugerido por defecto. No se usa para autorización;
    /// eso lo cubre el rol ESPECIALISTA por sí solo. Null para cualquier otro rol.
    /// </summary>
    public string? Especialidad { get; set; }

    public int? SchoolZoneId { get; set; }
    public SchoolZone? SchoolZone { get; set; }

    public int? AcademySubscriptionId { get; set; }
    public AcademySubscription? AcademySubscription { get; set; }
}
