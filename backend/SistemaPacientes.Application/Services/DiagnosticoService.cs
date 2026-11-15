using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;
using SistemaPacientes.Domain.Contracts;
using SistemaPacientes.Domain.Entities;

namespace SistemaPacientes.Application.Services;

public class DiagnosticoService : IDiagnosticoService
{
    private readonly IRepository<Diagnostico> _repository;
    private readonly IRepository<Consulta> _consultaRepository;
    private readonly IRepository<Tratamiento> _tratamientoRepository;

    public DiagnosticoService(
        IRepository<Diagnostico> repository,
        IRepository<Consulta> consultaRepository,
        IRepository<Tratamiento> tratamientoRepository)
    {
        _repository = repository;
        _consultaRepository = consultaRepository;
        _tratamientoRepository = tratamientoRepository;
    }

    public async Task<ServiceResult<IEnumerable<DiagnosticoResponseDto>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return ServiceResult<IEnumerable<DiagnosticoResponseDto>>.Ok(items.Select(ToResponse));
    }

    public async Task<ServiceResult<DiagnosticoResponseDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<DiagnosticoResponseDto>.NotFound();
        }

        return ServiceResult<DiagnosticoResponseDto>.Ok(ToResponse(item));
    }

    public async Task<ServiceResult<DiagnosticoResponseDto>> CreateAsync(DiagnosticoCreateDto dto)
    {
        var nuevo = ToEntity(dto);
        var error = await ValidarAsync(nuevo, null);
        if (error is not null)
        {
            return ServiceResult<DiagnosticoResponseDto>.Invalid(error);
        }

        await _repository.AddAsync(nuevo);

        return ServiceResult<DiagnosticoResponseDto>.Created(ToResponse(nuevo));
    }

    public async Task<ServiceResult<DiagnosticoResponseDto>> UpdateAsync(int id, DiagnosticoUpdateDto dto)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente is null)
        {
            return ServiceResult<DiagnosticoResponseDto>.NotFound();
        }

        var datos = ToEntity(dto);
        var error = await ValidarAsync(datos, id);
        if (error is not null)
        {
            return ServiceResult<DiagnosticoResponseDto>.Invalid(error);
        }

        Copiar(existente, datos);
        await _repository.UpdateAsync(existente);

        return ServiceResult<DiagnosticoResponseDto>.Ok(ToResponse(existente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        if (await _tratamientoRepository.ExistsAsync(x => x.DiagnosticoId == id))
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar el diagnóstico porque tiene tratamientos registrados.");
        }

        await _repository.DeleteAsync(item);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarAsync(Diagnostico e, int? excludeId)
    {
        if (e.Codigo != null && e.Codigo.Length > 20)
        {
            return "El código no puede superar los 20 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(e.Descripcion))
        {
            return "La descripción es obligatoria.";
        }

        if (e.Descripcion.Length < 3 || e.Descripcion.Length > 300)
        {
            return "La descripción debe tener entre 3 y 300 caracteres.";
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

    private static Diagnostico ToEntity(DiagnosticoCreateDto dto)
    {
        return new Diagnostico
        {
            ConsultaId = dto.ConsultaId,
            Codigo = string.IsNullOrWhiteSpace(dto.Codigo) ? null : dto.Codigo.Trim(),
            Descripcion = (dto.Descripcion ?? string.Empty).Trim()
        };
    }

    private static Diagnostico ToEntity(DiagnosticoUpdateDto dto)
    {
        return new Diagnostico
        {
            ConsultaId = dto.ConsultaId,
            Codigo = string.IsNullOrWhiteSpace(dto.Codigo) ? null : dto.Codigo.Trim(),
            Descripcion = (dto.Descripcion ?? string.Empty).Trim()
        };
    }

    private static void Copiar(Diagnostico destino, Diagnostico origen)
    {
        destino.ConsultaId = origen.ConsultaId;
        destino.Codigo = origen.Codigo;
        destino.Descripcion = origen.Descripcion;
    }

    private static DiagnosticoResponseDto ToResponse(Diagnostico item)
    {
        return new DiagnosticoResponseDto
        {
            Id = item.Id,
            ConsultaId = item.ConsultaId,
            Codigo = item.Codigo,
            Descripcion = item.Descripcion
        };
    }
}
