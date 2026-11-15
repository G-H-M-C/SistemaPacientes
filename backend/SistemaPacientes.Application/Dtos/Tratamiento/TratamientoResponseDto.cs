namespace SistemaPacientes.Application.Dtos;

public class TratamientoResponseDto
{
    public int Id { get; set; }
    public int DiagnosticoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
