using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddTutorRequest
{
    [Required]
    [MaxLength(200)]
    public string CompleteName { get; set; } = null!;

    [MaxLength(50)]
    public string? Parentesco { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    public string? Address { get; set; }

    public Guid? UserId { get; set; }
}
