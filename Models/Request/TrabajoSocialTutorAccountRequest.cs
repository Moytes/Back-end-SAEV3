using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class TrabajoSocialTutorAccountRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
}
