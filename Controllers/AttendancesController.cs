using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Attendances;
using unhsjm_asistencia_backend.DTOs.Attendances;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public sealed class AttendancesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly UpdateAttendanceDetailCommand _updateDetailCommand;

        public AttendancesController(
            ICommandInvoker commandInvoker,
            UpdateAttendanceDetailCommand updateDetailCommand)
        {
            _commandInvoker = commandInvoker;
            _updateDetailCommand = updateDetailCommand;
        }

        [HttpPatch("asistencias-detalle/{id:guid}")]
        [ProducesResponseType(typeof(AttendanceDetailResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAttendanceDetailAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAttendanceDetailDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateDetailCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}