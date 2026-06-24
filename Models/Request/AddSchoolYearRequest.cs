using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddSchoolYearRequest
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    public bool Activo { get; set; } = true;
}
