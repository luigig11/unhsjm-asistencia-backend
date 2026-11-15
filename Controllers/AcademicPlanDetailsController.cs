using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicPlanDetails;
using unhsjm_asistencia_backend.DTOs.AcademicPlanDetails;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/detalles-plan-academico")]
    [Authorize]
    public sealed class AcademicPlanDetailsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAcademicPlanDetailsCommand _listCommand;
        private readonly CreateAcademicPlanDetailCommand _createCommand;
        private readonly UpdateAcademicPlanDetailCommand _updateCommand;

        public AcademicPlanDetailsController(
            ICommandInvoker commandInvoker,
            ListAcademicPlanDetailsCommand listCommand,
            CreateAcademicPlanDetailCommand createCommand,
            UpdateAcademicPlanDetailCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicPlanDetailListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAcademicPlanDetailsAsync(
            [FromQuery(Name = "id_plan_academico")] Guid? academicPlanId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(academicPlanId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(AcademicPlanDetailDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAcademicPlanDetailAsync(
            [FromBody] CreateAcademicPlanDetailDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOrDirector")]
        [ProducesResponseType(typeof(AcademicPlanDetailDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAcademicPlanDetailAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAcademicPlanDetailDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}