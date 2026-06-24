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

    public int? SchoolZoneId { get; set; }
    public SchoolZone? SchoolZone { get; set; }

    public int? AcademySubscriptionId { get; set; }
    public AcademySubscription? AcademySubscription { get; set; }
}
