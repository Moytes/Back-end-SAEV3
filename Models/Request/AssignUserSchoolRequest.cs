using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignUserSchoolRequest
{
    [Required]
    public Guid SchoolId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }
}