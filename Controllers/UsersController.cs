using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Users;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.Commands.Users;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Authorize(Policy = "AdminOnly")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListUsersCommand _listUsersCommand;
        private readonly GetUserByIdCommand _getUserByIdCommand;
        private readonly UpdateUserCommand _updateUserCommand;
        private readonly DeleteUserCommand _deleteUserCommand;

        public UsersController(
            ICommandInvoker commandInvoker,
            ListUsersCommand listUsersCommand,
            GetUserByIdCommand getUserByIdCommand,
            UpdateUserCommand updateUserCommand,
            DeleteUserCommand deleteUserCommand)
        {
            _commandInvoker = commandInvoker;
            _listUsersCommand = listUsersCommand;
            _getUserByIdCommand = getUserByIdCommand;
            _updateUserCommand = updateUserCommand;
            _deleteUserCommand = deleteUserCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<UserListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync(
            [FromQuery] UserFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listUsersCommand.WithFilter(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getUserByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateUserDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateUserCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteUserCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
