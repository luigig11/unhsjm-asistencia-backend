using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicPeriods;
using unhsjm_asistencia_backend.DTOs.AcademicPeriods;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/periodos-academicos")]
    [Authorize]
    public sealed class AcademicPeriodsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAcademicPeriodsCommand _listCommand;
        private readonly CreateAcademicPeriodCommand _createCommand;
        private readonly UpdateAcademicPeriodCommand _updateCommand;
        private readonly DeleteAcademicPeriodCommand _deleteCommand;

        public AcademicPeriodsController(
            ICommandInvoker commandInvoker,
            ListAcademicPeriodsCommand listCommand,
            CreateAcademicPeriodCommand createCommand,
            UpdateAcademicPeriodCommand updateCommand,
            DeleteAcademicPeriodCommand deleteCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
            _deleteCommand = deleteCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicPeriodListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAcademicPeriodsAsync(
            [FromQuery(Name = "id_tipo_periodo_academico")] Guid? academicPeriodTypeId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(academicPeriodTypeId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicPeriodDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAcademicPeriodAsync(
            [FromBody] CreateAcademicPeriodDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicPeriodDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAcademicPeriodAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAcademicPeriodDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAcademicPeriodAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _commandInvoker.InvokeAsync(_deleteCommand.WithId(id), cancellationToken);
            return NoContent();
        }
    }
}

