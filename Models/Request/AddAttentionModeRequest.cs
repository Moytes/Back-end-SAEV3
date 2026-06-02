using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddAttentionModeRequest
{
    [Required]
    public Guid SchoolYearId { get; set; }

    [Required]
    public Phases Phase { get; set; }

    [Required]
    public AttentionTypes Type { get; set; }
}