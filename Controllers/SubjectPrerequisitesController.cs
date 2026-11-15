using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.SubjectPrerequisites;
using unhsjm_asistencia_backend.DTOs.SubjectPrerequisites;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/materias-prerequisitos")]
    [Authorize(Policy = "AdminOrDirector")]
    public sealed class SubjectPrerequisitesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly CreateSubjectPrerequisiteCommand _createCommand;
        private readonly DeleteSubjectPrerequisiteCommand _deleteCommand;

        public SubjectPrerequisitesController(
            ICommandInvoker commandInvoker,
            CreateSubjectPrerequisiteCommand createCommand,
            DeleteSubjectPrerequisiteCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _createCommand = createCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SubjectPrerequisiteDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSubjectPrerequisiteAsync(
            [FromBody] CreateSubjectPrerequisiteDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteSubjectPrerequisiteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}