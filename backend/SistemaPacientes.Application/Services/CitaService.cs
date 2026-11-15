using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _repository;
    private readonly IRepository<Paciente> _pacienteRepository;
    private readonly IRepository<Medico> _medicoRepository;

    public CitaService(
        ICitaRepository repository,
        IRepository<Paciente> pacienteRepository,
        IRepository<Medico> medicoRepository)
    {
        _repository = repository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository = medicoRepository;
    }

    public async Task<ServiceResult<IEnumerable<CitaResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<CitaResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<CitaResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<CitaResponseDto>.NotFound();
        }

        return ServiceResult<CitaResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<CitaResponseDto>> CreateAsync(CitaCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<CitaResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<CitaResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<CitaResponseDto>> UpdateAsync(int id, CitaUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<CitaResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<CitaResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<CitaResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _repository.HasConsultaAsync(id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar la cita porque tiene una consulta registrada.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Cita e, int? excludeId)
    {
        if (e.FechaHora == default)
        {
            return "La fecha y hora de la cita es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(e.Motivo))
        {
            return "El motivo es obligatorio.";
        }

        if (e.Motivo.Length < 3 || e.Motivo.Length > 200)
        {
            return "El motivo debe tener entre 3 y 200 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Estado))
        {
            return "El estado es obligatorio.";
        }

        if (e.Estado.Length > 20)
        {
            return "El estado no puede superar los 20 caracteres.";
        }

        if (e.Estado != "Programada" && e.Estado != "Completada" && e.Estado != "Cancelada")
        {
            return "El estado debe ser uno de los siguientes valores: Programada, Completada, Cancelada.";
        }

        if (e.PacienteId <= 0)
        {
            return "El Id del paciente debe ser mayor que 0.";
        }

        if (!await _pacienteRepository.ExistsAsync(x => x.Id == e.PacienteId))
        {
            return $"El paciente con Id {e.PacienteId} no existe.";
        }

        if (e.MedicoId <= 0)
        {
            return "El Id del médico debe ser mayor que 0.";
        }

        if (!await _medicoRepository.ExistsAsync(x => x.Id == e.MedicoId))
        {
            return $"El médico con Id {e.MedicoId} no existe.";
        }

        if (excludeId == null && e.FechaHora <= DateTime.Now)
        {
            return "La fecha y hora de la cita debe ser futura.";
        }

        if (e.Estado != "Cancelada" && await _repository.ExisteCitaEnHorarioAsync(e.MedicoId, e.FechaHora, excludeId))
        {
            return "El médico ya tiene una cita en ese horario.";
        }

        return null;
    }

    private static Cita ToEntity(CitaCreateDto dto)
    {
        return new Cita
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            FechaHora = dto.FechaHora,
            Motivo = (dto.Motivo ?? string.Empty).Trim(),
            Estado = (dto.Estado ?? string.Empty).Trim()
        };
    }

    private static Cita ToEntity(CitaUpdateDto dto)
    {
        return new Cita
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            FechaHora = dto.FechaHora,
            Motivo = (dto.Motivo ?? string.Empty).Trim(),
            Estado = (dto.Estado ?? string.Empty).Trim()
        };
    }

    private static void Copiar(Cita destino, Cita origen)
    {
        destino.PacienteId = origen.PacienteId;
        destino.MedicoId = origen.MedicoId;
        destino.FechaHora = origen.FechaHora;
        destino.Motivo = origen.Motivo;
        destino.Estado = origen.Estado;
    }

    private static CitaResponseDto ToResponse(Cita item)
    {
        return new CitaResponseDto
        {
            Id = item.Id,
            PacienteId = item.PacienteId,
            MedicoId = item.MedicoId,
            FechaHora = item.FechaHora,
            Motivo = item.Motivo,
            Estado = item.Estado
        };
    }
}
