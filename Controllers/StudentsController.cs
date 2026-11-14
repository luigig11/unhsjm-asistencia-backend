using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.Commands.Students;
using unhsjm_asistencia_backend.DTOs.Students;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/estudiantes")]
    [Authorize]
    public sealed class StudentsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListStudentsCommand _listStudentsCommand;
        private readonly GetStudentByIdCommand _getStudentByIdCommand;
        private readonly CreateStudentCommand _createStudentCommand;
        private readonly UpdateStudentCommand _updateStudentCommand;
        private readonly DeleteStudentCommand _deleteStudentCommand;

        public StudentsController(
            ICommandInvoker commandInvoker,
            ListStudentsCommand listStudentsCommand,
            GetStudentByIdCommand getStudentByIdCommand,
            CreateStudentCommand createStudentCommand,
            UpdateStudentCommand updateStudentCommand,
            DeleteStudentCommand deleteStudentCommand)
        {
            _commandInvoker = commandInvoker;
            _listStudentsCommand = listStudentsCommand;
            _getStudentByIdCommand = getStudentByIdCommand;
            _createStudentCommand = createStudentCommand;
            _updateStudentCommand = updateStudentCommand;
            _deleteStudentCommand = deleteStudentCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListStudentsAsync(
            [FromQuery] StudentFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listStudentsCommand.WithFilter(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getStudentByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateStudentAsync(
            [FromBody] CreateStudentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createStudentCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStudentAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateStudentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateStudentCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudentAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteStudentCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
