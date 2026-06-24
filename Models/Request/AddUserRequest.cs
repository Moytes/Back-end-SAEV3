using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddUserRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FatherLastName { get; set; } = null!;

    [MaxLength(100)]
    public string? MotherLastName { get; set; }

    [Required]
    public int RoleId { get; set; }

    public int? SchoolZoneId { get; set; }
    public int? AcademySubscriptionId { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }
}
