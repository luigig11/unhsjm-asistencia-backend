using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Asistencia;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard/resumen")]
        public async Task<ActionResult<DashboardResumenDto>> ObtenerResumenDashboard([FromQuery] Guid? idPeriodoEscolar)
        {
            var hoyUtc = DateTime.UtcNow;
            var hoyDateOnly = DateOnly.FromDateTime(hoyUtc);
            var inicioHoy = hoyUtc.Date;
            var finHoy = inicioHoy.AddDays(1).AddTicks(-1);

            var periodoActivo = idPeriodoEscolar.HasValue
                ? await _context.PeriodoEscolars.FirstOrDefaultAsync(p => p.Id == idPeriodoEscolar.Value)
                : await _context.PeriodoEscolars.FirstOrDefaultAsync(p => p.Activo && p.FechaInicio <= hoyDateOnly && p.FechaFin >= hoyDateOnly)
                  ?? await _context.PeriodoEscolars.OrderByDescending(p => p.FechaInicio).FirstOrDefaultAsync();

            var totalGrupos = await _context.Grupos.CountAsync(g => g.Activo);

            var sesionesHoyQuery = _context.Calendarios
                .Include(c => c.Asistencium)
                .Include(c => c.IdAulaNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdGrupoNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdMallaCurricularNavigation)
                        .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdDocenteNavigation)
                        .ThenInclude(d => d.IdUsuarioNavigation)
                .Where(c => c.Activo && c.FechaInicio >= inicioHoy && c.FechaInicio <= finHoy);

            var sesionesHoy = await sesionesHoyQuery
                .OrderBy(c => c.FechaInicio)
                .Select(c => new SesionHoyItemDto
                {
                    IdCalendario = c.Id,
                    MateriaNombre = c.IdMateriaPorGrupoNavigation.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    GrupoCodigo = c.IdMateriaPorGrupoNavigation.IdGrupoNavigation.Codigo,
                    AulaCodigo = c.IdAulaNavigation.Codigo,
                    DocenteNombre = $"{c.IdMateriaPorGrupoNavigation.IdDocenteNavigation.IdUsuarioNavigation.Nombre} {c.IdMateriaPorGrupoNavigation.IdDocenteNavigation.IdUsuarioNavigation.Apellido}",
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    EstadoActa = c.Asistencium == null
                        ? "PENDIENTE"
                        : (c.Asistencium.DocenteAsignadoAsistio == true
                            ? "PRESENTE"
                            : (c.Asistencium.IdDocenteSustituto != null ? "SUSTITUTO" : "AUSENTE"))
                })
                .ToListAsync();

            var alertasAusencia = sesionesHoy.Count(s => s.EstadoActa == "AUSENTE" || s.EstadoActa == "SUSTITUTO");

            var proximosEventos = await _context.Feriados
                .Include(f => f.IdTipoEventoNavigation)
                .Where(f => f.Activo && f.FechaFin >= inicioHoy)
                .OrderBy(f => f.FechaInicio)
                .Take(5)
                .Select(f => new EventoProximoItemDto
                {
                    IdFeriado = f.Id,
                    Nombre = f.Nombre,
                    TipoEvento = f.IdTipoEventoNavigation.Nombre,
                    FechaInicio = f.FechaInicio,
                    FechaFin = f.FechaFin
                })
                .ToListAsync();

            return Ok(new DashboardResumenDto
            {
                PeriodoEscolarActivo = periodoActivo != null ? new PeriodoActivoResumenDto
                {
                    Id = periodoActivo.Id,
                    Nombre = periodoActivo.NombrePeriodoEscolar
                } : null,
                TotalGruposOfertados = totalGrupos,
                TotalSesionesHoy = sesionesHoy.Count,
                AlertasAusenciaDocenteHoy = alertasAusencia,
                SesionesHoy = sesionesHoy,
                ProximosEventos = proximosEventos
            });
        }

        [HttpGet("docentes/me/sesiones-hoy")]
        public async Task<ActionResult<List<DocenteSesionHoyDto>>> ObtenerAgendaHoyDocente([FromQuery] Guid? idDocente)
        {
            var hoyUtc = DateTime.UtcNow;
            var inicioHoy = hoyUtc.Date;
            var finHoy = inicioHoy.AddDays(1).AddTicks(-1);

            var query = _context.Calendarios
                .Include(c => c.Asistencium)
                .Include(c => c.IdAulaNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdGrupoNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdMallaCurricularNavigation)
                        .ThenInclude(mc => mc.IdMateriaNavigation)
                .Where(c => c.Activo && c.FechaInicio >= inicioHoy && c.FechaInicio <= finHoy);

            if (idDocente.HasValue)
            {
                query = query.Where(c =>
                    c.IdMateriaPorGrupoNavigation.IdDocente == idDocente.Value ||
                    (c.Asistencium != null && c.Asistencium.IdDocenteSustituto == idDocente.Value));
            }

            var sesiones = await query
                .OrderBy(c => c.FechaInicio)
                .Select(c => new DocenteSesionHoyDto
                {
                    IdCalendario = c.Id,
                    IdMateriaPorGrupo = c.IdMateriaPorGrupo,
                    MateriaCodigo = c.IdMateriaPorGrupoNavigation.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = c.IdMateriaPorGrupoNavigation.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    GrupoCodigo = c.IdMateriaPorGrupoNavigation.IdGrupoNavigation.Codigo,
                    AulaCodigo = c.IdAulaNavigation.Codigo,
                    AulaUbicacion = c.IdAulaNavigation.Ubicacion,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    IdAsistencia = c.Asistencium != null ? c.Asistencium.Id : null,
                    ActaRegistrada = c.Asistencium != null,
                    EsSustituto = idDocente.HasValue && c.Asistencium != null && c.Asistencium.IdDocenteSustituto == idDocente.Value
                })
                .ToListAsync();

            return Ok(sesiones);
        }

        [HttpGet("docentes/me/cursos")]
        public async Task<ActionResult<List<DocenteCursoDto>>> ObtenerMisCursosDocente([FromQuery] Guid? idDocente, [FromQuery] Guid? idInstanciaAcademica)
        {
            var query = _context.MateriaPorGrupos
                .Include(mpg => mpg.IdGrupoNavigation)
                    .ThenInclude(g => g.IdInstanciaAcademicaNavigation)
                .Include(mpg => mpg.IdMallaCurricularNavigation)
                    .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(mpg => mpg.InscripcionGrupos)
                .Where(mpg => mpg.Activo);

            if (idDocente.HasValue)
            {
                query = query.Where(mpg => mpg.IdDocente == idDocente.Value);
            }

            if (idInstanciaAcademica.HasValue)
            {
                query = query.Where(mpg => mpg.IdGrupoNavigation.IdInstanciaAcademica == idInstanciaAcademica.Value);
            }

            var cursos = await query
                .OrderBy(mpg => mpg.IdGrupoNavigation.Codigo)
                .ThenBy(mpg => mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre)
                .Select(mpg => new DocenteCursoDto
                {
                    IdMateriaPorGrupo = mpg.Id,
                    InstanciaNombre = mpg.IdGrupoNavigation.IdInstanciaAcademicaNavigation.Nombre,
                    GrupoCodigo = mpg.IdGrupoNavigation.Codigo,
                    MateriaCodigo = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    TotalInscritos = mpg.InscripcionGrupos.Count(ig => ig.Activo),
                    CupoLimite = mpg.CupoLimite
                })
                .ToListAsync();

            return Ok(cursos);
        }
    }
}
