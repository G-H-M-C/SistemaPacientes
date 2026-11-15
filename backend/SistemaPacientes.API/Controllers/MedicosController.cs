using Microsoft.AspNetCore.Mvc;
using SistemaPacientes.Application.Contracts;
using SistemaPacientes.Application.Core;
using SistemaPacientes.Application.Dtos;

namespace SistemaPacientes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MedicoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MedicoResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MedicoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicoResponseDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result.Type == ResultType.NotFound)
        {
            return NotFound();
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MedicoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicoResponseDto>> Create(MedicoCreateDto dto)
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
    [ProducesResponseType(typeof(MedicoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicoResponseDto>> Update(int id, MedicoUpdateDto dto)
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
