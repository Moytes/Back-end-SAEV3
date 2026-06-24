using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignUserGroupRequest
{
    [Required]
    public int GroupId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public bool EsTitular { get; set; }
}
