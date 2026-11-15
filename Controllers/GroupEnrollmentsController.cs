using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.GroupEnrollments;
using unhsjm_asistencia_backend.DTOs.GroupEnrollments;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/inscripciones-grupo")]
    [Authorize]
    public sealed class GroupEnrollmentsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListGroupEnrollmentsCommand _listCommand;
        private readonly CreateGroupEnrollmentCommand _createCommand;
        private readonly UpdateGroupEnrollmentCommand _updateCommand;

        public GroupEnrollmentsController(
            ICommandInvoker commandInvoker,
            ListGroupEnrollmentsCommand listCommand,
            CreateGroupEnrollmentCommand createCommand,
            UpdateGroupEnrollmentCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<GroupEnrollmentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListGroupEnrollmentsAsync(
            [FromQuery(Name = "id_materia_por_grupo")] Guid? groupSubjectId,
            [FromQuery(Name = "id_estudiante")] Guid? studentId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(groupSubjectId, studentId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupEnrollmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupEnrollmentAsync(
            [FromBody] CreateGroupEnrollmentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupEnrollmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroupEnrollmentAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateGroupEnrollmentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}