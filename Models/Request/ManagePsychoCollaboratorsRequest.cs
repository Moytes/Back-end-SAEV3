using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class ManagePsychoCollaboratorsRequest
{
    [Required]
    [MinLength(1)]
    public List<PsychoCollaboratorItemRequest> Items { get; set; } = [];
}