using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Teachers;
using unhsjm_asistencia_backend.DTOs.Teachers;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/docentes")]
    [Authorize]
    public sealed class TeachersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListTeachersCommand _listTeachersCommand;
        private readonly GetTeacherByIdCommand _getTeacherByIdCommand;
        private readonly CreateTeacherCommand _createTeacherCommand;
        private readonly UpdateTeacherCommand _updateTeacherCommand;
        private readonly DeleteTeacherCommand _deleteTeacherCommand;

        public TeachersController(
            ICommandInvoker commandInvoker,
            ListTeachersCommand listTeachersCommand,
            GetTeacherByIdCommand getTeacherByIdCommand,
            CreateTeacherCommand createTeacherCommand,
            UpdateTeacherCommand updateTeacherCommand,
            DeleteTeacherCommand deleteTeacherCommand)
        {
            _commandInvoker = commandInvoker;
            _listTeachersCommand = listTeachersCommand;
            _getTeacherByIdCommand = getTeacherByIdCommand;
            _createTeacherCommand = createTeacherCommand;
            _updateTeacherCommand = updateTeacherCommand;
            _deleteTeacherCommand = deleteTeacherCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TeacherListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListTeachersAsync(
            [FromQuery] TeacherFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listTeachersCommand.WithFilter(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TeacherDetailDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeacherByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getTeacherByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTeacherAsync(
            [FromBody] CreateTeacherDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createTeacherCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTeacherAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateTeacherDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateTeacherCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTeacherAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteTeacherCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
