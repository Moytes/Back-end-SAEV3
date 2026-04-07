using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class UpdateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string FatherLastName { get; set; } = null!;

    public string? MotherLastName { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public Guid? SchoolZoneId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public boolStatus Status { get; set; }
}