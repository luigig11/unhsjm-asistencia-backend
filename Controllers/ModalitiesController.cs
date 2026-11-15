using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Modalities;
using unhsjm_asistencia_backend.DTOs.Modalities;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/modalidades")]
    [Authorize]
    public sealed class ModalitiesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListModalitiesCommand _listCommand;
        private readonly CreateModalityCommand _createCommand;
        private readonly UpdateModalityCommand _updateCommand;
        private readonly DeleteModalityCommand _deleteCommand;

        public ModalitiesController(
            ICommandInvoker commandInvoker,
            ListModalitiesCommand listCommand,
            CreateModalityCommand createCommand,
            UpdateModalityCommand updateCommand,
            DeleteModalityCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ModalityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListModalitiesAsync(
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(search, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ModalityDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateModalityAsync(
            [FromBody] CreateModalityDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ModalityDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateModalityAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateModalityDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteModalityAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
