using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class TratamientoUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id del diagnóstico debe ser mayor que 0.")]
    public int DiagnosticoId { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 300 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }
}
