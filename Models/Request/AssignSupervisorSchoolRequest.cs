using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignSupervisorSchoolRequest
{
    [Required]
    public int SchoolId { get; set; }
}
