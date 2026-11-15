using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Groups;
using unhsjm_asistencia_backend.DTOs.Groups;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/grupos")]
    [Authorize]
    public sealed class GroupsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListGroupsCommand _listCommand;
        private readonly CreateGroupCommand _createCommand;
        private readonly UpdateGroupCommand _updateCommand;

        public GroupsController(
            ICommandInvoker commandInvoker,
            ListGroupsCommand listCommand,
            CreateGroupCommand createCommand,
            UpdateGroupCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<GroupListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListGroupsAsync(
            [FromQuery(Name = "id_instancia_academica")] Guid? academicInstanceId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(academicInstanceId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupAsync(
            [FromBody] CreateGroupDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroupAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateGroupDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}