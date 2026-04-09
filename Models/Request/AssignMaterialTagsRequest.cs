using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AssignMaterialTagsRequest
{
    [Required]
    [MinLength(1)]
    public List<string> Tags { get; set; } = [];
}