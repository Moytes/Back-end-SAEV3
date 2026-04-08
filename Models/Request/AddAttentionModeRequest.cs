using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddAttentionModeRequest
{
    [Required]
    public Guid SchoolYearId { get; set; }

    [Required]
    public phases Phase { get; set; }

    [Required]
    public attentionTypes Type { get; set; }
}