using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class UpsertCIEAnswersRequest
{
    [Required]
    [MinLength(1)]
    public List<CIEAnswerItemRequest> Items { get; set; } = [];

    public evaluationStatus? Status { get; set; }
}