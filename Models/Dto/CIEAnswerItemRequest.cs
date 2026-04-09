using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class CIEAnswerItemRequest
{
    [Required]
    public Guid SubIndicatorId { get; set; }

    public bool? Achieved { get; set; }
    public short? HelpLevel { get; set; }
    public answerType? ResponseType { get; set; }
    public string? Observation { get; set; }
    public string? EvidenceUrl { get; set; }
}