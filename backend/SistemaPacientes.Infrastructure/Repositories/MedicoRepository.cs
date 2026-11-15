using Microsoft.EntityFrameworkCore;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;
using SistemaPacientes.Infrastructure.Context;

namespace SistemaPacientes.Infrastructure.Repositories;

public class MedicoRepository : Repository<Medico>, IMedicoRepository
{
    public MedicoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsLicenciaAsync(string licencia, int? excludeId = null)
    {
        return await _dbSet.AnyAsync(m => m.Licencia == licencia && (excludeId == null || m.Id != excludeId));
    }

    public async Task<bool> HasCitasAsync(int medicoId)
    {
        return await _context.Citas.AnyAsync(c => c.MedicoId == medicoId);
    }
}
