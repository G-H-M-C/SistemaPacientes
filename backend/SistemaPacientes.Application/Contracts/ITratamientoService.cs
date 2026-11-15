using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface ITratamientoService : ICrudService<TratamientoCreateDto, TratamientoUpdateDto, TratamientoResponseDto>
{
}
