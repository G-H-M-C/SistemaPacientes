using SistemaPacientes.Domain.Core;

namespace SistemaPacientes.Domain.Entities;

public class DetalleReceta : BaseEntity
{
    public int RecetaId { get; set; }
    public int MedicamentoId { get; set; }
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
}
