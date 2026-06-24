using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddGroupRequest
{
    [Required]
    public int SchoolId { get; set; }

    [Required]
    public int GradeId { get; set; }

    [Required]
    [MaxLength(1)]
    public string Section { get; set; } = "A";

    [Required]
    public int SchoolYearId { get; set; }
}
