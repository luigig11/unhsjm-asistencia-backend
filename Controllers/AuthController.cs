using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Models;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Auth;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly RegisterUserCommand _registerUserCommand;
        private readonly LoginCommand _loginCommand;
        private readonly RefreshTokenCommand _refreshTokenCommand;
        private readonly LogoutCommand _logoutCommand;
        private readonly GetCurrentProfileCommand _getCurrentProfileCommand;

        public AuthController(
            ICommandInvoker commandInvoker, 
            RegisterUserCommand registerUserCommand,
            LoginCommand loginCommand,
            RefreshTokenCommand refreshTokenCommand,
            LogoutCommand logoutCommand,
            GetCurrentProfileCommand getCurrentProfileCommand)
        {
            _commandInvoker = commandInvoker;
            _registerUserCommand = registerUserCommand;
            _loginCommand = loginCommand;
            _refreshTokenCommand = refreshTokenCommand;
            _logoutCommand = logoutCommand;
            _getCurrentProfileCommand = getCurrentProfileCommand;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_loginCommand.WithRequest(dto), cancellationToken);
            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_registerUserCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RefreshTokenResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshSessionAsync([FromBody] RefreshTokenRequestDto dto,CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_refreshTokenCommand.WithRequest(dto), cancellationToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_logoutCommand, cancellationToken);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(CurrentProfileResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentProfileAsync(CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getCurrentProfileCommand, cancellationToken);
            return Ok(result);
        }
    }
}
