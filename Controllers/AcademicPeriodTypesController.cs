using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicPeriodTypes;
using unhsjm_asistencia_backend.DTOs.AcademicPeriodTypes;

namespace plantilla.Controllers;

[ApiController]
[Route("api/v1/tipos-periodo-academico")]
[Authorize]
public sealed class AcademicPeriodTypesController : ControllerBase
{
    private readonly ICommandInvoker _commandInvoker;
    private readonly ListAcademicPeriodTypesCommand _listCommand;
    private readonly CreateAcademicPeriodTypeCommand _createCommand;
    private readonly UpdateAcademicPeriodTypeCommand _updateCommand;
    private readonly DeleteAcademicPeriodTypeCommand _deleteCommand;

    public AcademicPeriodTypesController(
        ICommandInvoker commandInvoker,
        ListAcademicPeriodTypesCommand listCommand,
        CreateAcademicPeriodTypeCommand createCommand,
        UpdateAcademicPeriodTypeCommand updateCommand,
        DeleteAcademicPeriodTypeCommand deleteCommand)
    {
        _commandInvoker = commandInvoker;
        _listCommand = listCommand;
        _createCommand = createCommand;
        _updateCommand = updateCommand;
        _deleteCommand = deleteCommand;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AcademicPeriodTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAcademicPeriodTypesAsync(
        [FromQuery(Name = "activo")] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(isActive), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(AcademicPeriodTypeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAcademicPeriodTypeAsync(
        [FromBody] CreateAcademicPeriodTypeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(AcademicPeriodTypeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAcademicPeriodTypeAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateAcademicPeriodTypeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAcademicPeriodTypeAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
        return NoContent();
    }
}

