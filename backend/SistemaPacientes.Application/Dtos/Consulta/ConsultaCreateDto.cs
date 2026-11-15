using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class ConsultaCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la cita debe ser mayor que 0.")]
    public int CitaId { get; set; }

    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La descripción de los síntomas es obligatoria.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "La descripción de los síntomas debe tener entre 3 y 500 caracteres.")]
    public string Sintomas { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
    public string? Observaciones { get; set; }
}
