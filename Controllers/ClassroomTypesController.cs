using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.ClassroomTypes;
using unhsjm_asistencia_backend.DTOs.ClassroomTypes;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/tipos-aula")]
    [Authorize]
    public sealed class ClassroomTypesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListClassroomTypesCommand _listCommand;
        private readonly CreateClassroomTypeCommand _createCommand;
        private readonly UpdateClassroomTypeCommand _updateCommand;

        public ClassroomTypesController(
            ICommandInvoker commandInvoker,
            ListClassroomTypesCommand listCommand,
            CreateClassroomTypeCommand createCommand,
            UpdateClassroomTypeCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ClassroomTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListClassroomTypesAsync(
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ClassroomTypeDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateClassroomTypeAsync(
            [FromBody] CreateClassroomTypeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ClassroomTypeDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassroomTypeAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateClassroomTypeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}