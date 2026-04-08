using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class StudentAttentionAreaAssignmentItemRequest
{
    [Required]
    public Guid AttentionAreaId { get; set; }

    public boolStatus IsRequired { get; set; } = boolStatus.True;
    public string? Notes { get; set; }
}
