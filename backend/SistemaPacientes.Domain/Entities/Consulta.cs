using SistemaPacientes.Domain.Core;

namespace SistemaPacientes.Domain.Entities;

public class Consulta : BaseEntity
{
    public int CitaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Sintomas { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
