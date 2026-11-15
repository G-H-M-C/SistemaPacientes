using SistemaPacientes.Domain.Core;

namespace SistemaPacientes.Domain.Entities;

public class Medico : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public int EspecialidadId { get; set; }
}
