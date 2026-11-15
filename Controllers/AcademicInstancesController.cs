using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicInstances;
using unhsjm_asistencia_backend.DTOs.AcademicInstances;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/instancias-academicas")]
    [Authorize]
    public sealed class AcademicInstancesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAcademicInstancesCommand _listCommand;
        private readonly CreateAcademicInstanceCommand _createCommand;
        private readonly UpdateAcademicInstanceCommand _updateCommand;

        public AcademicInstancesController(
            ICommandInvoker commandInvoker,
            ListAcademicInstancesCommand listCommand,
            CreateAcademicInstanceCommand createCommand,
            UpdateAcademicInstanceCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicInstanceListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAcademicInstancesAsync(
            [FromQuery(Name = "id_periodo_escolar")] Guid? schoolPeriodId,
            [FromQuery(Name = "id_plan_academico")] Guid? academicPlanId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(schoolPeriodId, academicPlanId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicInstanceDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAcademicInstanceAsync(
            [FromBody] CreateAcademicInstanceDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicInstanceDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAcademicInstanceAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAcademicInstanceDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}