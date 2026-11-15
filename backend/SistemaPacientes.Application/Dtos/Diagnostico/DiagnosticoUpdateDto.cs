using System.ComponentModel.DataAnnotations;

namespace SistemaPacientes.Application.Dtos;

public class DiagnosticoUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la consulta debe ser mayor que 0.")]
    public int ConsultaId { get; set; }

    [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres.")]
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 300 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}
