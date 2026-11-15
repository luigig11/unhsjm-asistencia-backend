using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Dashboard;
using unhsjm_asistencia_backend.DTOs.Dashboard;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    [Authorize(Policy = "AdminOrDirector")]
    public sealed class DashboardController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly GetDashboardSummaryCommand _summaryCommand;

        public DashboardController(
            ICommandInvoker commandInvoker,
            GetDashboardSummaryCommand summaryCommand)
        {
            _commandInvoker = commandInvoker;
            _summaryCommand = summaryCommand;
        }

        [HttpGet("resumen")]
        [ProducesResponseType(typeof(DashboardSummaryResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummaryAsync(
            [FromQuery(Name = "id_periodo_escolar")] Guid? schoolPeriodId,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_summaryCommand.WithSchoolPeriodId(schoolPeriodId), cancellationToken);
            return Ok(result);
        }
    }
}