using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IDetalleRecetaService : ICrudService<DetalleRecetaCreateDto, DetalleRecetaUpdateDto, DetalleRecetaResponseDto>
{
}
