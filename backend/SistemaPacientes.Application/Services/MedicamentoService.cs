using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class MedicamentoService : IMedicamentoService
{
    private readonly IRepository<Medicamento> _repository;
    private readonly IRepository<DetalleReceta> _detalleRecetaRepository;

    public MedicamentoService(
        IRepository<Medicamento> repository,
        IRepository<DetalleReceta> detalleRecetaRepository)
    {
        _repository = repository;
        _detalleRecetaRepository = detalleRecetaRepository;
    }

    public async Task<ServiceResult<IEnumerable<MedicamentoResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<MedicamentoResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<MedicamentoResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<MedicamentoResponseDto>.NotFound();
        }

        return ServiceResult<MedicamentoResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<MedicamentoResponseDto>> CreateAsync(MedicamentoCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<MedicamentoResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<MedicamentoResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<MedicamentoResponseDto>> UpdateAsync(int id, MedicamentoUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<MedicamentoResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<MedicamentoResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<MedicamentoResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _detalleRecetaRepository.ExistsAsync(x => x.MedicamentoId == id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar el medicamento porque está en recetas registradas.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Medicamento e, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(e.Nombre))
        {
            return "El nombre es obligatorio.";
        }

        if (e.Nombre.Length < 2 || e.Nombre.Length > 100)
        {
            return "El nombre debe tener entre 2 y 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Presentacion))
        {
            return "La presentación es obligatoria.";
        }

        if (e.Presentacion.Length < 2 || e.Presentacion.Length > 50)
        {
            return "La presentación debe tener entre 2 y 50 caracteres.";
        }

        if (e.Concentracion != null && e.Concentracion.Length > 50)
        {
            return "La concentración no puede superar los 50 caracteres.";
        }

        if (await _repository.ExistsAsync(x => x.Nombre == e.Nombre && (excludeId == null || x.Id != excludeId)))
        {
            return $"Ya existe un medicamento con el nombre {e.Nombre}.";
        }

        return null;
    }

    private static Medicamento ToEntity(MedicamentoCreateDto dto)
    {
        return new Medicamento
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Presentacion = (dto.Presentacion ?? string.Empty).Trim(),
            Concentracion = string.IsNullOrWhiteSpace(dto.Concentracion) ? null : dto.Concentracion.Trim()
        };
    }

    private static Medicamento ToEntity(MedicamentoUpdateDto dto)
    {
        return new Medicamento
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Presentacion = (dto.Presentacion ?? string.Empty).Trim(),
            Concentracion = string.IsNullOrWhiteSpace(dto.Concentracion) ? null : dto.Concentracion.Trim()
        };
    }

    private static void Copiar(Medicamento destino, Medicamento origen)
    {
        destino.Nombre = origen.Nombre;
        destino.Presentacion = origen.Presentacion;
        destino.Concentracion = origen.Concentracion;
    }

    private static MedicamentoResponseDto ToResponse(Medicamento item)
    {
        return new MedicamentoResponseDto
        {
            Id = item.Id,
            Nombre = item.Nombre,
            Presentacion = item.Presentacion,
            Concentracion = item.Concentracion
        };
    }
}
