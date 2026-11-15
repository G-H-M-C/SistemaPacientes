using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Domain.Contracts;

public interface IMedicoRepository : IRepository<Medico>
{
    Task<bool> ExistsLicenciaAsync(string licencia, int? excludeId = null);
    Task<bool> HasCitasAsync(int medicoId);
}
