using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IConsultaService : ICrudService<ConsultaCreateDto, ConsultaUpdateDto, ConsultaResponseDto>
{
}
