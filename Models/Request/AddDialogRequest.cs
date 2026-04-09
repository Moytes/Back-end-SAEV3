using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddDialogRequest
{
    [Required]
    public Guid MaterialId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public string? CharacterJson { get; set; }
    public string? ScenesJson { get; set; }

    [Range(0, short.MaxValue)]
    public short? EstimatedDurationMin { get; set; }
}