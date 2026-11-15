using Microsoft.AspNetCore.Mvc;
using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PacienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PacienteResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PacienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteResponseDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result.Type == ResultType.NotFound)
        {
            return NotFound();
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PacienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PacienteResponseDto>> Create(PacienteCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (result.Type == ResultType.Invalid)
        {
            return BadRequest(result.Error);
        }

        var creado = result.Data!;

        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PacienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteResponseDto>> Update(int id, PacienteUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result.Type == ResultType.NotFound)
        {
            return NotFound();
        }

        if (result.Type == ResultType.Invalid)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (result.Type == ResultType.NotFound)
        {
            return NotFound();
        }

        if (result.Type == ResultType.Invalid)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
