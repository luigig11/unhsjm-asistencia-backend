using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Days;
using unhsjm_asistencia_backend.DTOs.Days;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/dias")]
    [Authorize]
    public sealed class DaysController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly ListDaysCommand _listCommand;

        public DaysController(ICommandInvoker commandInvoker, ListDaysCommand listCommand)
        {
            _commandInvoker = commandInvoker;
            _listCommand = listCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DayDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListDaysAsync(CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_listCommand, cancellationToken);
            return Ok(result);
        }
    }
}