using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Attendances;
using unhsjm_asistencia_backend.Commands.CalendarSessions;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/calendarios")]
    [Authorize]
    public sealed class CalendarSessionsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListCalendarSessionsCommand _listCommand;
        private readonly CreateCalendarSessionCommand _createCommand;
        private readonly UpdateCalendarSessionCommand _updateCommand;
        private readonly GetSessionAttendanceSheetCommand _getAttendanceSheetCommand;
        private readonly SaveSessionAttendanceSheetCommand _saveAttendanceSheetCommand;

        public CalendarSessionsController(
            ICommandInvoker commandInvoker,
            ListCalendarSessionsCommand listCommand,
            CreateCalendarSessionCommand createCommand,
            UpdateCalendarSessionCommand updateCommand,
            GetSessionAttendanceSheetCommand getAttendanceSheetCommand,
            SaveSessionAttendanceSheetCommand saveAttendanceSheetCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _getAttendanceSheetCommand = getAttendanceSheetCommand;
            _saveAttendanceSheetCommand = saveAttendanceSheetCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CalendarSessionListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListCalendarSessionsAsync(
            [FromQuery(Name = "id_materia_por_grupo")] Guid? groupSubjectId,
            [FromQuery(Name = "fecha")] DateOnly? date,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(groupSubjectId, date, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(CalendarSessionDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCalendarSessionAsync(
            [FromBody] CreateCalendarSessionDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(CalendarSessionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCalendarSessionAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateCalendarSessionDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{idCalendario:guid}/acta-asistencia")]
        [ProducesResponseType(typeof(SessionAttendanceSheetDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceSheetAsync(
            [FromRoute] Guid idCalendario,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _getAttendanceSheetCommand.WithCalendarSessionId(idCalendario),
                cancellationToken);
            return Ok(result);
        }

        [HttpPut("{idCalendario:guid}/acta-asistencia")]
        [ProducesResponseType(typeof(SaveAttendanceSheetResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveAttendanceSheetAsync(
            [FromRoute] Guid idCalendario,
            [FromBody] SaveAttendanceSheetDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _saveAttendanceSheetCommand.WithRequest(idCalendario, dto),
                cancellationToken);
            return Ok(result);
        }
    }
}



