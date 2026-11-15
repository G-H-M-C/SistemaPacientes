using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class EspecialidadService : IEspecialidadService
{
    private readonly IRepository<Especialidad> _repository;
    private readonly IRepository<Medico> _medicoRepository;

    public EspecialidadService(
        IRepository<Especialidad> repository,
        IRepository<Medico> medicoRepository)
    {
        _repository = repository;
        _medicoRepository = medicoRepository;
    }

    public async Task<ServiceResult<IEnumerable<EspecialidadResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<EspecialidadResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<EspecialidadResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<EspecialidadResponseDto>.NotFound();
        }

        return ServiceResult<EspecialidadResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<EspecialidadResponseDto>> CreateAsync(EspecialidadCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<EspecialidadResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<EspecialidadResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<EspecialidadResponseDto>> UpdateAsync(int id, EspecialidadUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<EspecialidadResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<EspecialidadResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<EspecialidadResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _medicoRepository.ExistsAsync(x => x.EspecialidadId == id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar la especialidad porque tiene médicos registrados.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Especialidad e, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(e.Nombre))
        {
            return "El nombre es obligatorio.";
        }

        if (e.Nombre.Length < 3 || e.Nombre.Length > 100)
        {
            return "El nombre debe tener entre 3 y 100 caracteres.";
        }

        if (e.Descripcion != null && e.Descripcion.Length > 250)
        {
            return "La descripción no puede superar los 250 caracteres.";
        }

        if (await _repository.ExistsAsync(x => x.Nombre == e.Nombre && (excludeId == null || x.Id != excludeId)))
        {
            return $"Ya existe una especialidad con el nombre {e.Nombre}.";
        }

        return null;
    }

    private static Especialidad ToEntity(EspecialidadCreateDto dto)
    {
        return new Especialidad
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim()
        };
    }

    private static Especialidad ToEntity(EspecialidadUpdateDto dto)
    {
        return new Especialidad
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim()
        };
    }

    private static void Copiar(Especialidad destino, Especialidad origen)
    {
        destino.Nombre = origen.Nombre;
        destino.Descripcion = origen.Descripcion;
    }

    private static EspecialidadResponseDto ToResponse(Especialidad item)
    {
        return new EspecialidadResponseDto
        {
            Id = item.Id,
            Nombre = item.Nombre,
            Descripcion = item.Descripcion
        };
    }
}
