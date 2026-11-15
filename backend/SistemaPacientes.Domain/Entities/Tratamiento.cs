using SistemaPacientes.Domain.Core;

namespace SistemaPacientes.Domain.Entities;

public class Tratamiento : BaseEntity
{
    public int DiagnosticoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
