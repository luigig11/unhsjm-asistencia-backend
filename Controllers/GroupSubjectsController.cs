using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Attendances;
using unhsjm_asistencia_backend.Commands.CalendarSessions;
using unhsjm_asistencia_backend.Commands.GroupSubjects;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;
using unhsjm_asistencia_backend.DTOs.GroupSubjects;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/materias-por-grupo")]
    [Authorize]
    public sealed class GroupSubjectsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListGroupSubjectsCommand _listCommand;
        private readonly CreateGroupSubjectCommand _createCommand;
        private readonly UpdateGroupSubjectCommand _updateCommand;
        private readonly GenerateCalendarSessionsCommand _generateCalendarCommand;
        private readonly GetGroupAttendanceRosterCommand _getRosterCommand;

        public GroupSubjectsController(
            ICommandInvoker commandInvoker,
            ListGroupSubjectsCommand listCommand,
            CreateGroupSubjectCommand createCommand,
            UpdateGroupSubjectCommand updateCommand,
            GenerateCalendarSessionsCommand generateCalendarCommand,
            GetGroupAttendanceRosterCommand getRosterCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _generateCalendarCommand = generateCalendarCommand;
            _getRosterCommand = getRosterCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<GroupSubjectListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListGroupSubjectsAsync(
            [FromQuery(Name = "id_grupo")] Guid? groupId,
            [FromQuery(Name = "id_docente")] Guid? teacherId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(groupId, teacherId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupSubjectDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupSubjectAsync(
            [FromBody] CreateGroupSubjectDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupSubjectDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroupSubjectAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateGroupSubjectDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/generar-calendario")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GeneratedCalendarResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> GenerateCalendarSessionsAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_generateCalendarCommand.WithGroupSubjectId(id), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("{id:guid}/sabana-asistencia")]
        [ProducesResponseType(typeof(GroupAttendanceRosterDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceRosterAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _getRosterCommand.WithGroupSubjectId(id),
                cancellationToken);
            return Ok(result);
        }
    }
}