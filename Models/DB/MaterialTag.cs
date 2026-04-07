namespace Models.DB;

public class MaterialTag
{
    public Guid Id { get; set; }
    public string Tag { get; set; } = null!;

    // Material
    public Material Material { get; set; } = null!;
    public Guid MaterialId { get; set; }
}
