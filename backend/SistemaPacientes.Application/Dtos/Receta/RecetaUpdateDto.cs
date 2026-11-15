using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class RecetaUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la consulta debe ser mayor que 0.")]
    public int ConsultaId { get; set; }

    public DateTime Fecha { get; set; }

    [StringLength(500, ErrorMessage = "Las indicaciones no pueden superar los 500 caracteres.")]
    public string? Indicaciones { get; set; }
}
