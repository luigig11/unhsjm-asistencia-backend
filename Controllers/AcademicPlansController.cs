using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicPlans;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/planes-academicos")]
    [Authorize]
    public sealed class AcademicPlansController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListAcademicPlansCommand _listCommand;
        private readonly GetAcademicPlanByIdCommand _getByIdCommand;
        private readonly GetFullCurriculumGridCommand _getFullGridCommand;
        private readonly CreateAcademicPlanCommand _createCommand;
        private readonly UpdateAcademicPlanCommand _updateCommand;

        public AcademicPlansController(
            ICommandInvoker commandInvoker,
            ListAcademicPlansCommand listCommand,
            GetAcademicPlanByIdCommand getByIdCommand,
            GetFullCurriculumGridCommand getFullGridCommand,
            CreateAcademicPlanCommand createCommand,
            UpdateAcademicPlanCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _getByIdCommand = getByIdCommand;
            _getFullGridCommand = getFullGridCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicPlanListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAcademicPlansAsync(
            [FromQuery(Name = "id_direccion")] Guid? departmentId,
            [FromQuery(Name = "id_carrera")] Guid? careerId,
            [FromQuery(Name = "id_modalidad")] Guid? modalityId,
            [FromQuery(Name = "id_centro_universitario")] Guid? universityCenterId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(
                _listCommand.WithFilter(departmentId, careerId, modalityId, universityCenterId, isActive),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AcademicPlanDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAcademicPlanByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getByIdCommand.WithId(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}/malla-completa")]
        [ProducesResponseType(typeof(FullCurriculumGridDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFullCurriculumGridAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_getFullGridCommand.WithPlanId(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicPlanDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAcademicPlanAsync(
            [FromBody] CreateAcademicPlanDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(AcademicPlanDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAcademicPlanAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAcademicPlanDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}