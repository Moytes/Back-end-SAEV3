using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class StudentAttentionAreaAssignmentItemRequest
{
    [Required]
    public int AttentionAreaId { get; set; }

    public bool IsRequired { get; set; } = true;
    public string? Notes { get; set; }
}
