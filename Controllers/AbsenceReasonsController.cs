using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.AbsenceReasons;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AbsenceReasons;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/motivos-ausencia")]
    [Authorize]
    public sealed class AbsenceReasonsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAbsenceReasonsCommand _listCommand;
        private readonly CreateAbsenceReasonCommand _createCommand;
        private readonly UpdateAbsenceReasonCommand _updateCommand;

        public AbsenceReasonsController(
            ICommandInvoker commandInvoker,
            ListAbsenceReasonsCommand listCommand,
            CreateAbsenceReasonCommand createCommand,
            UpdateAbsenceReasonCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AbsenceReasonDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAbsenceReasonsAsync(
            [FromQuery(Name = "aplica_a")] string? appliesTo,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(appliesTo, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(AbsenceReasonDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAbsenceReasonAsync(
            [FromBody] CreateAbsenceReasonDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(AbsenceReasonDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAbsenceReasonAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAbsenceReasonDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}