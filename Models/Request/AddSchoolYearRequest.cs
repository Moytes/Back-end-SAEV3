using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddSchoolYearRequest
{
    [Required(ErrorMessage = "El nombre del ciclo es obligatorio")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
