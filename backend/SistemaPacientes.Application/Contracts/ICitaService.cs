using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface ICitaService : ICrudService<CitaCreateDto, CitaUpdateDto, CitaResponseDto>
{
}
