using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class MedicoUpdateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de licencia es obligatorio.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "El número de licencia debe tener entre 3 y 30 caracteres.")]
    public string Licencia { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string? Email { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El Id de la especialidad debe ser mayor que 0.")]
    public int EspecialidadId { get; set; }
}
