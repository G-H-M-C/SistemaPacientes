using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IMedicoService : ICrudService<MedicoCreateDto, MedicoUpdateDto, MedicoResponseDto>
{
}
