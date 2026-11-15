using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Carrers;
using unhsjm_asistencia_backend.DTOs.Carrers;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/carreras")]
    [Authorize]
    public sealed class CareersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListCareersCommand _listCommand;
        private readonly GetCareerByIdCommand _getByIdCommand;
        private readonly CreateCareerCommand _createCommand;
        private readonly UpdateCareerCommand _updateCommand;
        private readonly DeleteCareerCommand _deleteCommand;

        public CareersController(
            ICommandInvoker commandInvoker,
            ListCareersCommand listCommand,
            GetCareerByIdCommand getByIdCommand,
            CreateCareerCommand createCommand,
            UpdateCareerCommand updateCommand,
            DeleteCareerCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _getByIdCommand = getByIdCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CareerListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListCareersAsync(
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "id_direccion")] Guid? departmentId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(search, departmentId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CareerDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCareerByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(CareerDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCareerAsync(
            [FromBody] CreateCareerDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(CareerDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCareerAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateCareerDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCareerAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
