using SistemaPacientes.Application.Core;

namespace SistemaPacientes.Application.Contracts;

public interface ICrudService<TCreateDto, TUpdateDto, TResponseDto>
{
    Task<ServiceResult<IEnumerable<TResponseDto>>> GetAllAsync();
    Task<ServiceResult<TResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<TResponseDto>> CreateAsync(TCreateDto dto);
    Task<ServiceResult<TResponseDto>> UpdateAsync(int id, TUpdateDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
