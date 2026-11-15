using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class PacienteUpdateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de cédula es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El número de cédula debe tener entre 5 y 20 caracteres.")]
    public string Cedula { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El sexo es obligatorio.")]
    [StringLength(10, ErrorMessage = "El sexo no puede superar los 10 caracteres.")]
    [RegularExpression("^(Masculino|Femenino|Otro)$", ErrorMessage = "El sexo debe ser uno de los siguientes valores: Masculino, Femenino, Otro.")]
    public string Sexo { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
    public string? Direccion { get; set; }
}
