using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddSchoolZoneRequest
{
    [Required(ErrorMessage = "El número de zona es obligatorio")]
    public string Number { get; set; } = null!;

    [Required(ErrorMessage = "El CCT es obligatorio")]
    public string CCT { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }
}
