namespace Models.DB;

public class UserSchools
{
    public Guid Id { get; set; }

    // User
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

    // School
    public School School { get; set; } = null!;
    public Guid SchoolId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
