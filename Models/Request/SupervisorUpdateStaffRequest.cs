using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class SupervisorUpdateStaffRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

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

    [Required]
    public int SchoolId { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }
    public bool Activo { get; set; } = true;
}
