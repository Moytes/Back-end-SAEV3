namespace Models.Dto;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public int RoleId { get; set; }
    public string RoleClave { get; set; } = null!;
    public string RoleNombre { get; set; } = null!;
    public int? SchoolZoneId { get; set; }
    public int? SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
