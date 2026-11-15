namespace SistemaPacientes.Application.Dtos;

public class DetalleRecetaResponseDto
{
    public int Id { get; set; }
    public int RecetaId { get; set; }
    public int MedicamentoId { get; set; }
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
}
