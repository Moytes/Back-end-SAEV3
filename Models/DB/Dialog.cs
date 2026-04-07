namespace Models.DB;

public class Dialog
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CharacterJson { get; set; } // JSONB
    public string? ScenesJson { get; set; } // JSONB
    public short? EstimatedDurationMin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Material
    public Material Material { get; set; } = null!;
    public Guid MaterialId { get; set; }
}
