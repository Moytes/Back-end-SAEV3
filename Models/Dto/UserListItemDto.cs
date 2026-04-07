using Models.DB;

namespace Models.Dto;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FatherLastName { get; set; } = null!;
    public string? MotherLastName { get; set; }
    public UserRole Role { get; set; }
    public Guid? SchoolZoneId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public boolStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}