using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicCycles;
using unhsjm_asistencia_backend.DTOs.AcademicCycles;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/ciclos-academicos")]
    [Authorize]
    public sealed class AcademicCyclesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAcademicCyclesCommand _listCommand;
        private readonly CreateAcademicCycleCommand _createCommand;
        private readonly UpdateAcademicCycleCommand _updateCommand;
        private readonly DeleteAcademicCycleCommand _deleteCommand;

        public AcademicCyclesController(
            ICommandInvoker commandInvoker,
            ListAcademicCyclesCommand listCommand,
            CreateAcademicCycleCommand createCommand,
            UpdateAcademicCycleCommand updateCommand,
            DeleteAcademicCycleCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicCycleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAcademicCyclesAsync(
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicCycleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAcademicCycleAsync(
            [FromBody] CreateAcademicCycleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicCycleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAcademicCycleAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAcademicCycleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAcademicCycleAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}