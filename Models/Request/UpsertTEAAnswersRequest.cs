using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class UpsertTEAAnswersRequest
{
    [Required]
    [MinLength(1)]
    public List<TEAAnswerItemRequest> Items { get; set; } = [];
}