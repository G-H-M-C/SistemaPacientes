using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _repository;
    private readonly IRepository<Especialidad> _especialidadRepository;

    public MedicoService(
        IMedicoRepository repository,
        IRepository<Especialidad> especialidadRepository)
    {
        _repository = repository;
        _especialidadRepository = especialidadRepository;
    }

    public async Task<ServiceResult<IEnumerable<MedicoResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<MedicoResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<MedicoResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<MedicoResponseDto>.NotFound();
        }

        return ServiceResult<MedicoResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<MedicoResponseDto>> CreateAsync(MedicoCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<MedicoResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<MedicoResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<MedicoResponseDto>> UpdateAsync(int id, MedicoUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<MedicoResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<MedicoResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<MedicoResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _repository.HasCitasAsync(id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar el médico porque tiene citas registradas.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Medico e, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(e.Nombre))
        {
            return "El nombre es obligatorio.";
        }

        if (e.Nombre.Length < 2 || e.Nombre.Length > 100)
        {
            return "El nombre debe tener entre 2 y 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Apellido))
        {
            return "El apellido es obligatorio.";
        }

        if (e.Apellido.Length < 2 || e.Apellido.Length > 100)
        {
            return "El apellido debe tener entre 2 y 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Licencia))
        {
            return "El número de licencia es obligatorio.";
        }

        if (e.Licencia.Length < 3 || e.Licencia.Length > 30)
        {
            return "El número de licencia debe tener entre 3 y 30 caracteres.";
        }

        if (e.Telefono != null && e.Telefono.Length > 20)
        {
            return "El teléfono no puede superar los 20 caracteres.";
        }

        if (e.Email != null && e.Email.Length > 100)
        {
            return "El correo electrónico no puede superar los 100 caracteres.";
        }

        if (e.Email != null && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(e.Email))
        {
            return "El correo electrónico no es válido.";
        }

        if (e.EspecialidadId <= 0)
        {
            return "El Id de la especialidad debe ser mayor que 0.";
        }

        if (!await _especialidadRepository.ExistsAsync(x => x.Id == e.EspecialidadId))
        {
            return $"La especialidad con Id {e.EspecialidadId} no existe.";
        }

        if (await _repository.ExistsLicenciaAsync(e.Licencia, excludeId))
        {
            return $"Ya existe un médico con el número de licencia {e.Licencia}.";
        }

        return null;
    }

    private static Medico ToEntity(MedicoCreateDto dto)
    {
        return new Medico
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Apellido = (dto.Apellido ?? string.Empty).Trim(),
            Licencia = (dto.Licencia ?? string.Empty).Trim(),
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            EspecialidadId = dto.EspecialidadId
        };
    }

    private static Medico ToEntity(MedicoUpdateDto dto)
    {
        return new Medico
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Apellido = (dto.Apellido ?? string.Empty).Trim(),
            Licencia = (dto.Licencia ?? string.Empty).Trim(),
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            EspecialidadId = dto.EspecialidadId
        };
    }

    private static void Copiar(Medico destino, Medico origen)
    {
        destino.Nombre = origen.Nombre;
        destino.Apellido = origen.Apellido;
        destino.Licencia = origen.Licencia;
        destino.Telefono = origen.Telefono;
        destino.Email = origen.Email;
        destino.EspecialidadId = origen.EspecialidadId;
    }

    private static MedicoResponseDto ToResponse(Medico item)
    {
        return new MedicoResponseDto
        {
            Id = item.Id,
            Nombre = item.Nombre,
            Apellido = item.Apellido,
            Licencia = item.Licencia,
            Telefono = item.Telefono,
            Email = item.Email,
            EspecialidadId = item.EspecialidadId
        };
    }
}
