using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddTutorRequest
{
    [Required]
    [MaxLength(200)]
    public string CompleteName { get; set; } = null!;

    [MaxLength(100)]
    public string? Parent { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }
}