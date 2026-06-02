using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class StudentAttentionAreaAssignmentItemRequest
{
    [Required]
    public Guid AttentionAreaId { get; set; }

    public BoolStatus IsRequired { get; set; } = BoolStatus.True;
    public string? Notes { get; set; }
}
