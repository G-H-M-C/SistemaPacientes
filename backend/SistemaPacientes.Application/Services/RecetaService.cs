using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class RecetaService : IRecetaService
{
    private readonly IRepository<Receta> _repository;
    private readonly IRepository<Consulta> _consultaRepository;

    public RecetaService(
        IRepository<Receta> repository,
        IRepository<Consulta> consultaRepository)
    {
        _repository = repository;
        _consultaRepository = consultaRepository;
    }

    public async Task<ServiceResult<IEnumerable<RecetaResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<RecetaResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<RecetaResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<RecetaResponseDto>.NotFound();
        }

        return ServiceResult<RecetaResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<RecetaResponseDto>> CreateAsync(RecetaCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<RecetaResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<RecetaResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<RecetaResponseDto>> UpdateAsync(int id, RecetaUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<RecetaResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<RecetaResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<RecetaResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Receta e, int? excludeId)
    {
        if (e.Fecha == default)
        {
            return "La fecha de la receta es obligatoria.";
        }

        if (e.Indicaciones != null && e.Indicaciones.Length > 500)
        {
            return "Las indicaciones no pueden superar los 500 caracteres.";
        }

        if (e.ConsultaId <= 0)
        {
            return "El Id de la consulta debe ser mayor que 0.";
        }

        if (!await _consultaRepository.ExistsAsync(x => x.Id == e.ConsultaId))
        {
            return $"La consulta con Id {e.ConsultaId} no existe.";
        }

        return null;
    }

    private static Receta ToEntity(RecetaCreateDto dto)
    {
        return new Receta
        {
            ConsultaId = dto.ConsultaId,
            Fecha = dto.Fecha,
            Indicaciones = string.IsNullOrWhiteSpace(dto.Indicaciones) ? null : dto.Indicaciones.Trim()
        };
    }

    private static Receta ToEntity(RecetaUpdateDto dto)
    {
        return new Receta
        {
            ConsultaId = dto.ConsultaId,
            Fecha = dto.Fecha,
            Indicaciones = string.IsNullOrWhiteSpace(dto.Indicaciones) ? null : dto.Indicaciones.Trim()
        };
    }

    private static void Copiar(Receta destino, Receta origen)
    {
        destino.ConsultaId = origen.ConsultaId;
        destino.Fecha = origen.Fecha;
        destino.Indicaciones = origen.Indicaciones;
    }

    private static RecetaResponseDto ToResponse(Receta item)
    {
        return new RecetaResponseDto
        {
            Id = item.Id,
            ConsultaId = item.ConsultaId,
            Fecha = item.Fecha,
            Indicaciones = item.Indicaciones
        };
    }
}
