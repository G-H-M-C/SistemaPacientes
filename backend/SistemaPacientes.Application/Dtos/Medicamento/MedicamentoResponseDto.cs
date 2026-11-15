namespace SistemaPacientes.Application.Dtos;

public class MedicamentoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Presentacion { get; set; } = string.Empty;
    public string? Concentracion { get; set; }
}
