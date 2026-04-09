using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class UpsertCIEPhonologyAnswersRequest
{
    [Required]
    [MinLength(1)]
    public List<CIEPhonologyAnswerItemRequest> Items { get; set; } = [];

    public evaluationStatus? Status { get; set; }
}