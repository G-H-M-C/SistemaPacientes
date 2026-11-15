using Microsoft.EntityFrameworkCore;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;
using SistemaPacientes.Infrastructure.Context;

namespace SistemaPacientes.Infrastructure.Repositories;

public class PacienteRepository : Repository<Paciente>, IPacienteRepository
{
    public PacienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsCedulaAsync(string cedula, int? excludeId = null)
    {
        return await _dbSet.AnyAsync(p => p.Cedula == cedula && (excludeId == null || p.Id != excludeId));
    }

    public async Task<bool> HasCitasAsync(int pacienteId)
    {
        return await _context.Citas.AnyAsync(c => c.PacienteId == pacienteId);
    }
}
