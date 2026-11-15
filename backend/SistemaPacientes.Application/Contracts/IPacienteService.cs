using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IPacienteService : ICrudService<PacienteCreateDto, PacienteUpdateDto, PacienteResponseDto>
{
}
