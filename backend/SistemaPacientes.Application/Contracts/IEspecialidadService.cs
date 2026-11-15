using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IEspecialidadService : ICrudService<EspecialidadCreateDto, EspecialidadUpdateDto, EspecialidadResponseDto>
{
}
