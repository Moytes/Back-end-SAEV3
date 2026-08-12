using System.ComponentModel.DataAnnotations;

namespace Models.Request;

/// <summary>
/// Autoedición de perfil — a diferencia de <see cref="UpdateUserRequest"/> (uso exclusivo
/// de ADMIN), aquí solo se exponen los campos que cualquier usuario puede cambiar de sí
/// mismo. Rol, zona escolar, suscripción y estado activo se quedan como están.
/// </summary>
public class UpdateMyProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FatherLastName { get; set; } = null!;

    [MaxLength(100)]
    public string? MotherLastName { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }
}
