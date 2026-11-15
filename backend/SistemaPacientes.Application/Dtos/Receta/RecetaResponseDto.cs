namespace SistemaPacientes.Application.Dtos;

public class RecetaResponseDto
{
    public int Id { get; set; }
    public int ConsultaId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Indicaciones { get; set; }
}
