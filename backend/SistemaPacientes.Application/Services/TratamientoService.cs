using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class TratamientoService : ITratamientoService
{
    private readonly IRepository<Tratamiento> _repository;
    private readonly IRepository<Diagnostico> _diagnosticoRepository;

    public TratamientoService(
        IRepository<Tratamiento> repository,
        IRepository<Diagnostico> diagnosticoRepository)
    {
        _repository = repository;
        _diagnosticoRepository = diagnosticoRepository;
    }

    public async Task<ServiceResult<IEnumerable<TratamientoResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<TratamientoResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<TratamientoResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<TratamientoResponseDto>.NotFound();
        }

        return ServiceResult<TratamientoResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<TratamientoResponseDto>> CreateAsync(TratamientoCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<TratamientoResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<TratamientoResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<TratamientoResponseDto>> UpdateAsync(int id, TratamientoUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<TratamientoResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<TratamientoResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<TratamientoResponseDto>.Ok(ToResponse(existente));
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

    private async Task<string?> ValidarAsync(Tratamiento e, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(e.Descripcion))
        {
            return "La descripción es obligatoria.";
        }

        if (e.Descripcion.Length < 3 || e.Descripcion.Length > 300)
        {
            return "La descripción debe tener entre 3 y 300 caracteres.";
        }

        if (e.FechaInicio == default)
        {
            return "La fecha de inicio es obligatoria.";
        }

        if (e.DiagnosticoId <= 0)
        {
            return "El Id del diagnóstico debe ser mayor que 0.";
        }

        if (!await _diagnosticoRepository.ExistsAsync(x => x.Id == e.DiagnosticoId))
        {
            return $"El diagnóstico con Id {e.DiagnosticoId} no existe.";
        }

        if (e.FechaFin != null && e.FechaFin < e.FechaInicio)
        {
            return "La fecha final no puede ser anterior a la fecha de inicio.";
        }

        return null;
    }

    private static Tratamiento ToEntity(TratamientoCreateDto dto)
    {
        return new Tratamiento
        {
            DiagnosticoId = dto.DiagnosticoId,
            Descripcion = (dto.Descripcion ?? string.Empty).Trim(),
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };
    }

    private static Tratamiento ToEntity(TratamientoUpdateDto dto)
    {
        return new Tratamiento
        {
            DiagnosticoId = dto.DiagnosticoId,
            Descripcion = (dto.Descripcion ?? string.Empty).Trim(),
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };
    }

    private static void Copiar(Tratamiento destino, Tratamiento origen)
    {
        destino.DiagnosticoId = origen.DiagnosticoId;
        destino.Descripcion = origen.Descripcion;
        destino.FechaInicio = origen.FechaInicio;
        destino.FechaFin = origen.FechaFin;
    }

    private static TratamientoResponseDto ToResponse(Tratamiento item)
    {
        return new TratamientoResponseDto
        {
            Id = item.Id,
            DiagnosticoId = item.DiagnosticoId,
            Descripcion = item.Descripcion,
            FechaInicio = item.FechaInicio,
            FechaFin = item.FechaFin
        };
    }
}
