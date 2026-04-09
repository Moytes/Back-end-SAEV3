using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class UpdateCanalizationStatusRequest
{
    [Required]
    public canalizationStatus Status { get; set; }

    public DateOnly? ReceivedDate { get; set; }
}