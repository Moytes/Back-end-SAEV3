using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class TrabajoSocialAssignDocenteRequest
{
    [Required]
    public Guid DocenteId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public bool EsTitular { get; set; }
}
