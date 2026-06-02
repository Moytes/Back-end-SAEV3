using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddGroupRequest
{
    [Required]
    public Guid SchoolId { get; set; }

    [Required]
    public Grades Grade { get; set; }

    [Required]
    [MaxLength(50)]
    public string Section { get; set; } = "A";

    [Required]
    public Guid SchoolYearId { get; set; }
}
