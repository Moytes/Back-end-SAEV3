using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class ManagePsychoBapsRequest
{
    [Required]
    [MinLength(1)]
    public List<PsychoBapItemRequest> Items { get; set; } = [];
}