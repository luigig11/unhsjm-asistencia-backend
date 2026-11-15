using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.WeeklySchedules;
using unhsjm_asistencia_backend.DTOs.WeeklySchedules;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/calendarios-semanales")]
    [Authorize]
    public sealed class WeeklySchedulesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListWeeklySchedulesCommand _listCommand;
        private readonly CreateWeeklyScheduleCommand _createCommand;
        private readonly UpdateWeeklyScheduleCommand _updateCommand;

        public WeeklySchedulesController(
            ICommandInvoker commandInvoker,
            ListWeeklySchedulesCommand listCommand,
            CreateWeeklyScheduleCommand createCommand,
            UpdateWeeklyScheduleCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<WeeklyScheduleListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListWeeklySchedulesAsync(
            [FromQuery(Name = "id_materia_por_grupo")] Guid? groupSubjectId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(groupSubjectId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(WeeklyScheduleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateWeeklyScheduleAsync(
            [FromBody] CreateWeeklyScheduleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(WeeklyScheduleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWeeklyScheduleAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateWeeklyScheduleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}



