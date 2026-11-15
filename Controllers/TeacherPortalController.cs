using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.TeacherPortal;
using unhsjm_asistencia_backend.DTOs.TeacherPortal;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/docentes/me")]
    [Authorize(Policy = "TeacherOnly")]
    public sealed class TeacherPortalController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly GetMyTodaySessionsCommand _todaySessionsCommand;
        private readonly GetMyCoursesCommand _coursesCommand;

        public TeacherPortalController(
            ICommandInvoker commandInvoker,
            GetMyTodaySessionsCommand todaySessionsCommand,
            GetMyCoursesCommand coursesCommand)
        {
            _commandInvoker = commandInvoker;
            _todaySessionsCommand = todaySessionsCommand;
            _coursesCommand = coursesCommand;
        }

        [HttpGet("sesiones-hoy")]
        [ProducesResponseType(typeof(IReadOnlyList<TeacherTodaySessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodaySessionsAsync(CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_todaySessionsCommand, cancellationToken);
            return Ok(result);
        }

        [HttpGet("cursos")]
        [ProducesResponseType(typeof(IReadOnlyList<TeacherCourseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesAsync(
            [FromQuery(Name = "id_instancia_academica")] Guid? academicInstanceId,
            CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_coursesCommand.WithAcademicInstanceId(academicInstanceId), cancellationToken);
            return Ok(result);
        }
    }
}