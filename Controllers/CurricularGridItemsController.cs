using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.CurricularGridItems;
using unhsjm_asistencia_backend.DTOs.CurricularGridItems;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/mallas-curriculares")]
    [Authorize(Policy = "AdminOrDirector")]
    public sealed class CurricularGridItemsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly CreateCurricularGridItemCommand _createCommand;
        private readonly UpdateCurricularGridItemCommand _updateCommand;

        public CurricularGridItemsController(
            ICommandInvoker commandInvoker,
            CreateCurricularGridItemCommand createCommand,
            UpdateCurricularGridItemCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CurricularGridItemDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCurricularGridItemAsync(
            [FromBody] CreateCurricularGridItemDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CurricularGridItemDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCurricularGridItemAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateCurricularGridItemDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}