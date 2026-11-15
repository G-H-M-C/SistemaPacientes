using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class MedicamentoCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La presentación es obligatoria.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La presentación debe tener entre 2 y 50 caracteres.")]
    public string Presentacion { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La concentración no puede superar los 50 caracteres.")]
    public string? Concentracion { get; set; }
}
