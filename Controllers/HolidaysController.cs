using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Holidays;
using unhsjm_asistencia_backend.DTOs.Holidays;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/feriados")]
    [Authorize]
    public sealed class HolidaysController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListHolidaysCommand _listCommand;
        private readonly CreateHolidayCommand _createCommand;
        private readonly UpdateHolidayCommand _updateCommand;

        public HolidaysController(
            ICommandInvoker commandInvoker,
            ListHolidaysCommand listCommand,
            CreateHolidayCommand createCommand,
            UpdateHolidayCommand updateCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
            _createCommand = createCommand;
            _updateCommand = updateCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<HolidayListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListHolidaysAsync(
            [FromQuery(Name = "id_periodo_escolar")] Guid? schoolPeriodId,
            [FromQuery(Name = "id_tipo_evento")] Guid? eventTypeId,
            [FromQuery(Name = "activo")] bool? isActive,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand.WithFilter(schoolPeriodId, eventTypeId, isActive), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateHolidayAsync(
            [FromBody] CreateHolidayDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_createCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateHolidayAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateHolidayDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_updateCommand.WithRequest(id, dto), cancellationToken);
            return Ok(result);
        }
    }

}