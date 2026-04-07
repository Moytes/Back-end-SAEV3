using System.ComponentModel.DataAnnotations;

namespace Models.DB;

public enum materialTypeCatalog
{
    [Display(Name = "Diálogo animado interactivo")]
    DIALOGO_ANIMADO = 1,
    [Display(Name = "Actividad didáctica")]
    ACTIVIDAD = 2,
    [Display(Name = "Juego digital educativo")]
    JUEGO_DIGITAL = 3,
    [Display(Name = "Material visual / imagen")]
    IMAGEN = 4,
    [Display(Name = "Material de audio")]
    AUDIO = 5,
    [Display(Name = "Video educativo")]
    VIDEO = 6,
    [Display(Name = "Documento / ficha de trabajo")]
    DOCUMENTO = 7
}

public class MaterialType
{
    public Guid Id { get; set; }
    public string CVE { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
