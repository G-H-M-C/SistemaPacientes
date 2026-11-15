using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class CitaUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id del paciente debe ser mayor que 0.")]
    public int PacienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El Id del médico debe ser mayor que 0.")]
    public int MedicoId { get; set; }

    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres.")]
    [RegularExpression("^(Programada|Completada|Cancelada)$", ErrorMessage = "El estado debe ser uno de los siguientes valores: Programada, Completada, Cancelada.")]
    public string Estado { get; set; } = "Programada";
}
