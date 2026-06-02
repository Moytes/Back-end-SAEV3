using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(100)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Father last name is required")]
    [MaxLength(100)]
    public string FatherLastName { get; set; } = null!;

    [MaxLength(100)]
    public string? MotherLastName { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public UserRole Role { get; set; }

    public Guid? SchoolZoneId { get; set; }

    public Guid? SchoolId { get; set; }

    public Guid? SchoolYearId { get; set; }

    public Guid? StudentId { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
}
