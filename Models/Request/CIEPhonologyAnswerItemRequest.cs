using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class CIEPhonologyAnswerItemRequest
{
    [Required]
    public Guid SubIndicatorId { get; set; }

    public bool? Functional { get; set; }
    public string? ObservationForm { get; set; }
}