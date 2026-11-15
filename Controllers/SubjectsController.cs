using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Subjects;
using unhsjm_asistencia_backend.DTOs.Subjects;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/materias")]
    [Authorize]
    public sealed class SubjectsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListSubjectsCommand _listCommand;
        private readonly CreateSubjectCommand _createCommand;
        private readonly UpdateSubjectCommand _updateCommand;
        private readonly DeleteSubjectCommand _deleteCommand;

        public SubjectsController(
            ICommandInvoker commandInvoker,
            ListSubjectsCommand listCommand,
            CreateSubjectCommand createCommand,
            UpdateSubjectCommand updateCommand,
            DeleteSubjectCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<SubjectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListSubjectsAsync(
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(search, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSubjectAsync(
            [FromBody] CreateSubjectDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSubjectAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateSubjectDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteSubjectAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}

