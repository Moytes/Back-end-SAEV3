namespace Models.DB;

public class Tutor
{
    public int Id { get; set; }
    public string CompleteName { get; set; } = null!;
    public string? Parentesco { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
