namespace Models.Dto;

public class TutorListItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string CompleteName { get; set; } = null!;
    public string? Parent { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}