using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<IEnumerable<PacienteResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<PacienteResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<PacienteResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<PacienteResponseDto>.NotFound();
        }

        return ServiceResult<PacienteResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<PacienteResponseDto>> CreateAsync(PacienteCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<PacienteResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<PacienteResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<PacienteResponseDto>> UpdateAsync(int id, PacienteUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<PacienteResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<PacienteResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<PacienteResponseDto>.Ok(ToResponse(existente));
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
            return ServiceResult<bool>.Invalid("No se puede eliminar el paciente porque tiene citas registradas.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Paciente e, int? excludeId)
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

        if (string.IsNullOrWhiteSpace(e.Cedula))
        {
            return "El número de cédula es obligatorio.";
        }

        if (e.Cedula.Length < 5 || e.Cedula.Length > 20)
        {
            return "El número de cédula debe tener entre 5 y 20 caracteres.";
        }

        if (e.FechaNacimiento == default)
        {
            return "La fecha de nacimiento es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(e.Sexo))
        {
            return "El sexo es obligatorio.";
        }

        if (e.Sexo.Length > 10)
        {
            return "El sexo no puede superar los 10 caracteres.";
        }

        if (e.Sexo != "Masculino" && e.Sexo != "Femenino" && e.Sexo != "Otro")
        {
            return "El sexo debe ser uno de los siguientes valores: Masculino, Femenino, Otro.";
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

        if (e.Direccion != null && e.Direccion.Length > 200)
        {
            return "La dirección no puede superar los 200 caracteres.";
        }

        if (await _repository.ExistsCedulaAsync(e.Cedula, excludeId))
        {
            return $"Ya existe un paciente con el número de cédula {e.Cedula}.";
        }

        if (e.FechaNacimiento.Date > DateTime.Today)
        {
            return "La fecha de nacimiento no puede ser futura.";
        }

        return null;
    }

    private static Paciente ToEntity(PacienteCreateDto dto)
    {
        return new Paciente
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Apellido = (dto.Apellido ?? string.Empty).Trim(),
            Cedula = (dto.Cedula ?? string.Empty).Trim(),
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = (dto.Sexo ?? string.Empty).Trim(),
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim()
        };
    }

    private static Paciente ToEntity(PacienteUpdateDto dto)
    {
        return new Paciente
        {
            Nombre = (dto.Nombre ?? string.Empty).Trim(),
            Apellido = (dto.Apellido ?? string.Empty).Trim(),
            Cedula = (dto.Cedula ?? string.Empty).Trim(),
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = (dto.Sexo ?? string.Empty).Trim(),
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim()
        };
    }

    private static void Copiar(Paciente destino, Paciente origen)
    {
        destino.Nombre = origen.Nombre;
        destino.Apellido = origen.Apellido;
        destino.Cedula = origen.Cedula;
        destino.FechaNacimiento = origen.FechaNacimiento;
        destino.Sexo = origen.Sexo;
        destino.Telefono = origen.Telefono;
        destino.Email = origen.Email;
        destino.Direccion = origen.Direccion;
    }

    private static PacienteResponseDto ToResponse(Paciente item)
    {
        return new PacienteResponseDto
        {
            Id = item.Id,
            Nombre = item.Nombre,
            Apellido = item.Apellido,
            Cedula = item.Cedula,
            FechaNacimiento = item.FechaNacimiento,
            Sexo = item.Sexo,
            Telefono = item.Telefono,
            Email = item.Email,
            Direccion = item.Direccion
        };
    }
}
