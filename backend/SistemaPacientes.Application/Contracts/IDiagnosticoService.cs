using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.Application.Contracts;

public interface IDiagnosticoService : ICrudService<DiagnosticoCreateDto, DiagnosticoUpdateDto, DiagnosticoResponseDto>
{
}
