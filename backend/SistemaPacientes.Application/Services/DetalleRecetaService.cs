using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class DetalleRecetaService : IDetalleRecetaService
{
    private readonly IRepository<DetalleReceta> _repository;
    private readonly IRepository<Receta> _recetaRepository;
    private readonly IRepository<Medicamento> _medicamentoRepository;

    public DetalleRecetaService(
        IRepository<DetalleReceta> repository,
        IRepository<Receta> recetaRepository,
        IRepository<Medicamento> medicamentoRepository)
    {
        _repository = repository;
        _recetaRepository = recetaRepository;
        _medicamentoRepository = medicamentoRepository;
    }

    public async Task<ServiceResult<IEnumerable<DetalleRecetaResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<DetalleRecetaResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<DetalleRecetaResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<DetalleRecetaResponseDto>.NotFound();
        }

        return ServiceResult<DetalleRecetaResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<DetalleRecetaResponseDto>> CreateAsync(DetalleRecetaCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<DetalleRecetaResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<DetalleRecetaResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<DetalleRecetaResponseDto>> UpdateAsync(int id, DetalleRecetaUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<DetalleRecetaResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<DetalleRecetaResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<DetalleRecetaResponseDto>.Ok(ToResponse(existente));
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

    private async Task<string?> ValidarAsync(DetalleReceta e, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(e.Dosis))
        {
            return "La dosis es obligatoria.";
        }

        if (e.Dosis.Length < 1 || e.Dosis.Length > 100)
        {
            return "La dosis debe tener entre 1 y 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Frecuencia))
        {
            return "La frecuencia es obligatoria.";
        }

        if (e.Frecuencia.Length < 1 || e.Frecuencia.Length > 100)
        {
            return "La frecuencia debe tener entre 1 y 100 caracteres.";
        }

        if (e.DuracionDias < 1 || e.DuracionDias > 365)
        {
            return "La duración en días debe estar entre 1 y 365.";
        }

        if (e.RecetaId <= 0)
        {
            return "El Id de la receta debe ser mayor que 0.";
        }

        if (!await _recetaRepository.ExistsAsync(x => x.Id == e.RecetaId))
        {
            return $"La receta con Id {e.RecetaId} no existe.";
        }

        if (e.MedicamentoId <= 0)
        {
            return "El Id del medicamento debe ser mayor que 0.";
        }

        if (!await _medicamentoRepository.ExistsAsync(x => x.Id == e.MedicamentoId))
        {
            return $"El medicamento con Id {e.MedicamentoId} no existe.";
        }

        return null;
    }

    private static DetalleReceta ToEntity(DetalleRecetaCreateDto dto)
    {
        return new DetalleReceta
        {
            RecetaId = dto.RecetaId,
            MedicamentoId = dto.MedicamentoId,
            Dosis = (dto.Dosis ?? string.Empty).Trim(),
            Frecuencia = (dto.Frecuencia ?? string.Empty).Trim(),
            DuracionDias = dto.DuracionDias
        };
    }

    private static DetalleReceta ToEntity(DetalleRecetaUpdateDto dto)
    {
        return new DetalleReceta
        {
            RecetaId = dto.RecetaId,
            MedicamentoId = dto.MedicamentoId,
            Dosis = (dto.Dosis ?? string.Empty).Trim(),
            Frecuencia = (dto.Frecuencia ?? string.Empty).Trim(),
            DuracionDias = dto.DuracionDias
        };
    }

    private static void Copiar(DetalleReceta destino, DetalleReceta origen)
    {
        destino.RecetaId = origen.RecetaId;
        destino.MedicamentoId = origen.MedicamentoId;
        destino.Dosis = origen.Dosis;
        destino.Frecuencia = origen.Frecuencia;
        destino.DuracionDias = origen.DuracionDias;
    }

    private static DetalleRecetaResponseDto ToResponse(DetalleReceta item)
    {
        return new DetalleRecetaResponseDto
        {
            Id = item.Id,
            RecetaId = item.RecetaId,
            MedicamentoId = item.MedicamentoId,
            Dosis = item.Dosis,
            Frecuencia = item.Frecuencia,
            DuracionDias = item.DuracionDias
        };
    }
}
