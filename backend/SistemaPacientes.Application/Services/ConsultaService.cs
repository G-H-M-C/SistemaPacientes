using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class ConsultaService : IConsultaService
{
    private readonly IRepository<Consulta> _repository;
    private readonly IRepository<Cita> _citaRepository;
    private readonly IRepository<Diagnostico> _diagnosticoRepository;
    private readonly IRepository<Receta> _recetaRepository;

    public ConsultaService(
        IRepository<Consulta> repository,
        IRepository<Cita> citaRepository,
        IRepository<Diagnostico> diagnosticoRepository,
        IRepository<Receta> recetaRepository)
    {
        _repository = repository;
        _citaRepository = citaRepository;
        _diagnosticoRepository = diagnosticoRepository;
        _recetaRepository = recetaRepository;
    }

    public async Task<ServiceResult<IEnumerable<ConsultaResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<ConsultaResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<ConsultaResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<ConsultaResponseDto>.NotFound();
        }

        return ServiceResult<ConsultaResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<ConsultaResponseDto>> CreateAsync(ConsultaCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<ConsultaResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<ConsultaResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<ConsultaResponseDto>> UpdateAsync(int id, ConsultaUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<ConsultaResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<ConsultaResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<ConsultaResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _diagnosticoRepository.ExistsAsync(x => x.ConsultaId == id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar la consulta porque tiene diagnósticos registrados.");
        }

        if (await _recetaRepository.ExistsAsync(x => x.ConsultaId == id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar la consulta porque tiene recetas registradas.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Consulta e, int? excludeId)
    {
        if (e.Fecha == default)
        {
            return "La fecha de la consulta es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(e.Sintomas))
        {
            return "La descripción de los síntomas es obligatoria.";
        }

        if (e.Sintomas.Length < 3 || e.Sintomas.Length > 500)
        {
            return "La descripción de los síntomas debe tener entre 3 y 500 caracteres.";
        }

        if (e.Observaciones != null && e.Observaciones.Length > 500)
        {
            return "Las observaciones no pueden superar los 500 caracteres.";
        }

        if (e.CitaId <= 0)
        {
            return "El Id de la cita debe ser mayor que 0.";
        }

        if (!await _citaRepository.ExistsAsync(x => x.Id == e.CitaId))
        {
            return $"La cita con Id {e.CitaId} no existe.";
        }

        if (e.Fecha > DateTime.Now)
        {
            return "La fecha de la consulta no puede ser futura.";
        }

        if (await _repository.ExistsAsync(x => x.CitaId == e.CitaId && (excludeId == null || x.Id != excludeId)))
        {
            return "La cita ya tiene una consulta registrada.";
        }

        return null;
    }

    private static Consulta ToEntity(ConsultaCreateDto dto)
    {
        return new Consulta
        {
            CitaId = dto.CitaId,
            Fecha = dto.Fecha,
            Sintomas = (dto.Sintomas ?? string.Empty).Trim(),
            Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim()
        };
    }

    private static Consulta ToEntity(ConsultaUpdateDto dto)
    {
        return new Consulta
        {
            CitaId = dto.CitaId,
            Fecha = dto.Fecha,
            Sintomas = (dto.Sintomas ?? string.Empty).Trim(),
            Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim()
        };
    }

    private static void Copiar(Consulta destino, Consulta origen)
    {
        destino.CitaId = origen.CitaId;
        destino.Fecha = origen.Fecha;
        destino.Sintomas = origen.Sintomas;
        destino.Observaciones = origen.Observaciones;
    }

    private static ConsultaResponseDto ToResponse(Consulta item)
    {
        return new ConsultaResponseDto
        {
            Id = item.Id,
            CitaId = item.CitaId,
            Fecha = item.Fecha,
            Sintomas = item.Sintomas,
            Observaciones = item.Observaciones
        };
    }
}
