using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class TEAAnswerItemRequest
{
    [Required]
    public Guid IndicatorId { get; set; }

    public short? Frequency { get; set; }
    public short? Intensity { get; set; }
    public string? Observation { get; set; }
}