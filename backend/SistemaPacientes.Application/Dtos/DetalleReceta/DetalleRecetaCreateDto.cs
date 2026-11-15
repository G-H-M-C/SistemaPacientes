using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class DetalleRecetaCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la receta debe ser mayor que 0.")]
    public int RecetaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El Id del medicamento debe ser mayor que 0.")]
    public int MedicamentoId { get; set; }

    [Required(ErrorMessage = "La dosis es obligatoria.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "La dosis debe tener entre 1 y 100 caracteres.")]
    public string Dosis { get; set; } = string.Empty;

    [Required(ErrorMessage = "La frecuencia es obligatoria.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "La frecuencia debe tener entre 1 y 100 caracteres.")]
    public string Frecuencia { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "La duración en días debe estar entre 1 y 365.")]
    public int DuracionDias { get; set; }
}
