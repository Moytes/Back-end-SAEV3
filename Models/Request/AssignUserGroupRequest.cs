using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AssignUserGroupRequest
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public BoolStatus EsTitular { get; set; } = BoolStatus.False;
}