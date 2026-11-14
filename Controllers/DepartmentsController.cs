using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Departments;
using unhsjm_asistencia_backend.DTOs.Departments;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/direcciones")]
    [Authorize]
    public sealed class DepartmentsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListDepartmentsCommand _listCommand;
        private readonly GetDepartmentByIdCommand _getByIdCommand;
        private readonly CreateDepartmentCommand _createCommand;
        private readonly UpdateDepartmentCommand _updateCommand;
        private readonly DeleteDepartmentCommand _deleteCommand;

        public DepartmentsController(
            ICommandInvoker commandInvoker,
            ListDepartmentsCommand listCommand,
            GetDepartmentByIdCommand getByIdCommand,
            CreateDepartmentCommand createCommand,
            UpdateDepartmentCommand updateCommand,
            DeleteDepartmentCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _getByIdCommand = getByIdCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListDepartmentsAsync(
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(search, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartmentByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateDepartmentAsync(
            [FromBody] CreateDepartmentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDepartmentAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateDepartmentDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDepartmentAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}
