namespace Models.Dto;

public class TutorListItemDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public string CompleteName { get; set; } = null!;
    public string? Parentesco { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public Guid? UserId { get; set; }
}
