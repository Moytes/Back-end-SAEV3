namespace Models.DB;

public class UserGroups
{
    public int Id { get; set; }
    public bool EsTitular { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public int SchoolYearId { get; set; }
    public SchoolYear SchoolYear { get; set; } = null!;
}
