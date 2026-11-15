using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Classrooms;
using unhsjm_asistencia_backend.DTOs.Classrooms;
    
namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/aulas")]
    [Authorize]
    public sealed class ClassroomsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListClassroomsCommand _listCommand;
        private readonly CreateClassroomCommand _createCommand;
        private readonly UpdateClassroomCommand _updateCommand;

        public ClassroomsController(
            ICommandInvoker commandInvoker,
            ListClassroomsCommand listCommand,
            CreateClassroomCommand createCommand,
            UpdateClassroomCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ClassroomListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListClassroomsAsync(
            [FromQuery(Name = "id_tipo_aula")] Guid? classroomTypeId,
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _listCommand.WithFilter(classroomTypeId, search, isActive),
                cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ClassroomDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateClassroomAsync(
            [FromBody] CreateClassroomDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ClassroomDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassroomAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateClassroomDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}