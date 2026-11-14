using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Permissions;
using unhsjm_asistencia_backend.DTOs.Permissions;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/permisos")]
    [Authorize]
    public sealed class PermissionsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListPermissionsCommand _listPermissionsCommand;
        private readonly CreatePermissionCommand _createPermissionCommand;
        private readonly UpdatePermissionCommand _updatePermissionCommand;
        private readonly DeletePermissionCommand _deletePermissionCommand;

        public PermissionsController(
            ICommandInvoker commandInvoker,
            ListPermissionsCommand listPermissionsCommand,
            CreatePermissionCommand createPermissionCommand,
            UpdatePermissionCommand updatePermissionCommand,
            DeletePermissionCommand deletePermissionCommand)
        {
            _commandInvoker = commandInvoker;
            _listPermissionsCommand = listPermissionsCommand;
            _createPermissionCommand = createPermissionCommand;
            _updatePermissionCommand = updatePermissionCommand;
            _deletePermissionCommand = deletePermissionCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListPermissionsAsync(
            [FromQuery(Name = "id_rol")] Guid? roleId,
            [FromQuery(Name = "entidad")] string? entityName,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _listPermissionsCommand.WithFilter(roleId, entityName, isActive),
                cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePermissionAsync(
            [FromBody] CreatePermissionDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createPermissionCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePermissionAsync(
            [FromRoute] Guid id,
            [FromBody] UpdatePermissionDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updatePermissionCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePermissionAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deletePermissionCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
