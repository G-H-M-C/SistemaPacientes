using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Domain.Contracts;

public interface IPacienteRepository : IRepository<Paciente>
{
    Task<bool> ExistsCedulaAsync(string cedula, int? excludeId = null);
    Task<bool> HasCitasAsync(int pacienteId);
}
