using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Roles;
using unhsjm_asistencia_backend.DTOs.Roles;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    [Authorize]
    public sealed class RolesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListRolesCommand _listRolesCommand;
        private readonly CreateRoleCommand _createRoleCommand;
        private readonly UpdateRoleCommand _updateRoleCommand;
        private readonly DeleteRoleCommand _deleteRoleCommand;

        public RolesController(
            ICommandInvoker commandInvoker,
            ListRolesCommand listRolesCommand,
            CreateRoleCommand createRoleCommand,
            UpdateRoleCommand updateRoleCommand,
            DeleteRoleCommand deleteRoleCommand)
        {
            _commandInvoker = commandInvoker;
            _listRolesCommand = listRolesCommand;
            _createRoleCommand = createRoleCommand;
            _updateRoleCommand = updateRoleCommand;
            _deleteRoleCommand = deleteRoleCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListRolesAsync(
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listRolesCommand.WithFilter(isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateRoleAsync(
            [FromBody] CreateRoleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createRoleCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRoleAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateRoleDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateRoleCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRoleAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteRoleCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
