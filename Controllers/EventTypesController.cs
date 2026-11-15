using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.EventTypes;
using unhsjm_asistencia_backend.DTOs.EventTypes;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/tipos-evento")]
    [Authorize]
    public sealed class EventTypesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListEventTypesCommand _listCommand;
        private readonly CreateEventTypeCommand _createCommand;
        private readonly UpdateEventTypeCommand _updateCommand;

        public EventTypesController(
            ICommandInvoker commandInvoker,
            ListEventTypesCommand listCommand,
            CreateEventTypeCommand createCommand,
            UpdateEventTypeCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<EventTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListEventTypesAsync(
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(EventTypeDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEventTypeAsync(
            [FromBody] CreateEventTypeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(EventTypeDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEventTypeAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateEventTypeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}