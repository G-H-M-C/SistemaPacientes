using Microsoft.EntityFrameworkCore;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;
using SistemaPacientes.Infrastructure.Context;

namespace SistemaPacientes.Infrastructure.Repositories;

public class CitaRepository : Repository<Cita>, ICitaRepository
{
    public CitaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora, int? excludeId = null)
    {
        return await _dbSet.AnyAsync(c => c.MedicoId == medicoId
            && c.FechaHora == fechaHora
            && c.Estado != "Cancelada"
            && (excludeId == null || c.Id != excludeId));
    }

    public async Task<bool> HasConsultaAsync(int citaId)
    {
        return await _context.Consultas.AnyAsync(c => c.CitaId == citaId);
    }
}
