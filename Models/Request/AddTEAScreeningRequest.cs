using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddTEAScreeningRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid EvaluatorId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public DateOnly? ScreeningDate { get; set; }
    public string? ObservationContext { get; set; }
    public string? GeneralObservations { get; set; }
    public bool? RequiresCanalization { get; set; }
}