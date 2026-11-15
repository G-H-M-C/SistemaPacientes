namespace SistemaPacientes.Application.Dtos;

public class DiagnosticoResponseDto
{
    public int Id { get; set; }
    public int ConsultaId { get; set; }
    public string? Codigo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
