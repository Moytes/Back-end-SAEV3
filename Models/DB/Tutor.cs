namespace Models.DB;

public class Tutor
{
    public Guid Id { get; set; }
    public string CompleteName { get; set; } = null!;
    public string? Parent { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email {  get; set; }
    public string? Address { get; set; }

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId {  get; set; }
}
