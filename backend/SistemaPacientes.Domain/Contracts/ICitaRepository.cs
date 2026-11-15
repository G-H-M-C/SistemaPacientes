using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Domain.Contracts;

public interface ICitaRepository : IRepository<Cita>
{
    Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora, int? excludeId = null);
    Task<bool> HasConsultaAsync(int citaId);
}
