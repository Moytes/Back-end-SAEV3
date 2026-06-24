using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignUserSchoolRequest
{
    [Required]
    public int SchoolId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }
}
