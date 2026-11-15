using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.UniversityCenters;
using unhsjm_asistencia_backend.DTOs.UniversityCenters;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/centros-universitarios")]
    [Authorize]
    public sealed class UniversityCentersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListUniversityCentersCommand _listCommand;
        private readonly CreateUniversityCenterCommand _createCommand;
        private readonly UpdateUniversityCenterCommand _updateCommand;

        public UniversityCentersController(
            ICommandInvoker commandInvoker,
            ListUniversityCentersCommand listCommand,
            CreateUniversityCenterCommand createCommand,
            UpdateUniversityCenterCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<UniversityCenterDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUniversityCentersAsync(
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(search, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(UniversityCenterDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUniversityCenterAsync(
            [FromBody] CreateUniversityCenterDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(UniversityCenterDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUniversityCenterAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateUniversityCenterDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}