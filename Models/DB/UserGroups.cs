namespace Models.DB;

public class UserGroups
{
    public Guid Id { get; set; }
    public BoolStatus EsTitular { get; set; } = BoolStatus.False;

    // User
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

    // Group
    public Group Group { get; set; } = null!;
    public Guid GroupId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
