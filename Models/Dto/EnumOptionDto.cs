namespace Models.Dto;

public class EnumOptionDto
{
    public int Key { get; set; }
    public string Value { get; set; } = null!;
    public string? Label { get; set; }
}