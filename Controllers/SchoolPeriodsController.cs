using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.SchoolPeriods;
using unhsjm_asistencia_backend.DTOs.SchoolPeriods;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/periodos-escolares")]
    [Authorize]
    public sealed class SchoolPeriodsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListSchoolPeriodsCommand _listCommand;
        private readonly CreateSchoolPeriodCommand _createCommand;
        private readonly UpdateSchoolPeriodCommand _updateCommand;

        public SchoolPeriodsController(
            ICommandInvoker commandInvoker,
            ListSchoolPeriodsCommand listCommand,
            CreateSchoolPeriodCommand createCommand,
            UpdateSchoolPeriodCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<SchoolPeriodDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListSchoolPeriodsAsync(
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(SchoolPeriodDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSchoolPeriodAsync(
            [FromBody] CreateSchoolPeriodDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(SchoolPeriodDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSchoolPeriodAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateSchoolPeriodDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }
}