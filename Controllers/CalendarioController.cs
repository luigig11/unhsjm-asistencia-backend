using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Calendario;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CalendarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CalendarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("periodos-escolares")]
        public async Task<ActionResult<List<PeriodoEscolarDto>>> ListarPeriodosEscolares([FromQuery] bool? activo)
        {
            var query = _context.PeriodoEscolars.AsNoTracking().AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }

            var items = await query
                .OrderByDescending(p => p.FechaInicio)
                .Select(p => new PeriodoEscolarDto
                {
                    Id = p.Id,
                    NombrePeriodoEscolar = p.NombrePeriodoEscolar,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("feriados")]
        public async Task<ActionResult<List<FeriadoDto>>> ListarFeriados([FromQuery] Guid? idPeriodoEscolar, [FromQuery] Guid? idTipoEvento, [FromQuery] bool? activo)
        {
            var query = _context.Feriados
                .Include(f => f.IdTipoEventoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idPeriodoEscolar.HasValue)
            {
                query = query.Where(f => f.IdPeriodoEscolar == idPeriodoEscolar.Value);
            }

            if (idTipoEvento.HasValue)
            {
                query = query.Where(f => f.IdTipoEvento == idTipoEvento.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(f => f.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(f => f.FechaInicio)
                .Select(f => new FeriadoDto
                {
                    Id = f.Id,
                    IdPeriodoEscolar = f.IdPeriodoEscolar,
                    IdTipoEvento = f.IdTipoEvento,
                    TipoEventoCodigo = f.IdTipoEventoNavigation.Codigo,
                    TipoEventoNombre = f.IdTipoEventoNavigation.Nombre,
                    Nombre = f.Nombre,
                    FechaInicio = f.FechaInicio,
                    FechaFin = f.FechaFin,
                    Activo = f.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("instancias-academicas")]
        public async Task<ActionResult<List<InstanciaAcademicaDto>>> ListarInstanciasAcademicas([FromQuery] Guid? idPeriodoEscolar, [FromQuery] Guid? idPlanAcademico, [FromQuery] bool? activo)
        {
            var query = _context.InstanciaAcademicas
                .Include(i => i.IdPeriodoEscolarNavigation)
                .Include(i => i.IdDetallePlanAcademicoNavigation)
                    .ThenInclude(d => d.IdPlanAcademicoNavigation)
                .Include(i => i.IdDetallePlanAcademicoNavigation)
                    .ThenInclude(d => d.IdCicloAcademicoNavigation)
                .Include(i => i.IdDetallePlanAcademicoNavigation)
                    .ThenInclude(d => d.IdPeriodoAcademicoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idPeriodoEscolar.HasValue)
            {
                query = query.Where(i => i.IdPeriodoEscolar == idPeriodoEscolar.Value);
            }

            if (idPlanAcademico.HasValue)
            {
                query = query.Where(i => i.IdDetallePlanAcademicoNavigation.IdPlanAcademico == idPlanAcademico.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(i => i.Activo == activo.Value);
            }

            var items = await query
                .OrderByDescending(i => i.FechaInicio)
                .Select(i => new InstanciaAcademicaDto
                {
                    Id = i.Id,
                    IdPeriodoEscolar = i.IdPeriodoEscolar,
                    NombrePeriodoEscolar = i.IdPeriodoEscolarNavigation.NombrePeriodoEscolar,
                    IdDetallePlanAcademico = i.IdDetallePlanAcademico,
                    PlanCodigo = i.IdDetallePlanAcademicoNavigation.IdPlanAcademicoNavigation.Codigo,
                    CicloNombre = i.IdDetallePlanAcademicoNavigation.IdCicloAcademicoNavigation.Nombre,
                    PeriodoNombre = i.IdDetallePlanAcademicoNavigation.IdPeriodoAcademicoNavigation.Nombre,
                    Nombre = i.Nombre,
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    Activo = i.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("grupos")]
        public async Task<ActionResult<List<GrupoDto>>> ListarGrupos([FromQuery] Guid? idInstanciaAcademica, [FromQuery] bool? activo)
        {
            var query = _context.Grupos
                .Include(g => g.IdInstanciaAcademicaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idInstanciaAcademica.HasValue)
            {
                query = query.Where(g => g.IdInstanciaAcademica == idInstanciaAcademica.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(g => g.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(g => g.Codigo)
                .Select(g => new GrupoDto
                {
                    Id = g.Id,
                    IdInstanciaAcademica = g.IdInstanciaAcademica,
                    InstanciaNombre = g.IdInstanciaAcademicaNavigation.Nombre,
                    Codigo = g.Codigo,
                    Nombre = g.Nombre,
                    Cupo = g.Cupo,
                    Activo = g.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("materias-por-grupo")]
        public async Task<ActionResult<List<MateriaPorGrupoDto>>> ListarMateriasPorGrupo([FromQuery] Guid? idGrupo, [FromQuery] Guid? idDocente, [FromQuery] bool? activo)
        {
            var query = _context.MateriaPorGrupos
                .Include(m => m.IdGrupoNavigation)
                .Include(m => m.IdMallaCurricularNavigation)
                    .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(m => m.IdDocenteNavigation)
                    .ThenInclude(d => d.IdUsuarioNavigation)
                .Include(m => m.InscripcionGrupos)
                .AsNoTracking()
                .AsQueryable();

            if (idGrupo.HasValue)
            {
                query = query.Where(m => m.IdGrupo == idGrupo.Value);
            }

            if (idDocente.HasValue)
            {
                query = query.Where(m => m.IdDocente == idDocente.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(m => m.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(m => m.IdGrupoNavigation.Codigo)
                .Select(m => new MateriaPorGrupoDto
                {
                    Id = m.Id,
                    IdGrupo = m.IdGrupo,
                    GrupoCodigo = m.IdGrupoNavigation.Codigo,
                    IdMallaCurricular = m.IdMallaCurricular,
                    IdMateria = m.IdMallaCurricularNavigation.IdMateria,
                    MateriaCodigo = m.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = m.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    IdDocente = m.IdDocente,
                    DocenteNombreCompleto = $"{m.IdDocenteNavigation.IdUsuarioNavigation.Nombre} {m.IdDocenteNavigation.IdUsuarioNavigation.Apellido}",
                    CupoLimite = m.CupoLimite,
                    TotalInscritos = m.InscripcionGrupos.Count(ig => ig.Activo),
                    Activo = m.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("calendarios-semanales")]
        public async Task<ActionResult<List<CalendarioSemanalDto>>> ListarCalendarioSemanal([FromQuery] Guid idMateriaPorGrupo, [FromQuery] bool? activo)
        {
            var query = _context.CalendarioSemanals
                .Include(cs => cs.IdAulaNavigation)
                .Include(cs => cs.IdDiaNavigation)
                .Where(cs => cs.IdMateriaPorGrupo == idMateriaPorGrupo)
                .AsNoTracking()
                .AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(cs => cs.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(cs => cs.IdDia)
                .ThenBy(cs => cs.HoraInicio)
                .Select(cs => new CalendarioSemanalDto
                {
                    Id = cs.Id,
                    IdMateriaPorGrupo = cs.IdMateriaPorGrupo,
                    IdAula = cs.IdAula,
                    AulaCodigo = cs.IdAulaNavigation.Codigo,
                    AulaNombre = cs.IdAulaNavigation.Nombre,
                    IdDia = cs.IdDia,
                    DiaNombre = cs.IdDiaNavigation.Nombre,
                    HoraInicio = cs.HoraInicio,
                    HoraFin = cs.HoraFin,
                    Activo = cs.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("calendarios")]
        public async Task<ActionResult<List<SesionCalendarioDto>>> ListarSesionesCalendario([FromQuery] Guid? idMateriaPorGrupo, [FromQuery] DateOnly? fecha, [FromQuery] bool? activo)
        {
            var query = _context.Calendarios
                .Include(c => c.IdAulaNavigation)
                .Include(c => c.Asistencium)
                .AsNoTracking()
                .AsQueryable();

            if (idMateriaPorGrupo.HasValue)
            {
                query = query.Where(c => c.IdMateriaPorGrupo == idMateriaPorGrupo.Value);
            }

            if (fecha.HasValue)
            {
                var inicio = fecha.Value.ToDateTime(TimeOnly.MinValue);
                var fin = fecha.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(c => c.FechaInicio >= inicio && c.FechaInicio <= fin);
            }

            if (activo.HasValue)
            {
                query = query.Where(c => c.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(c => c.FechaInicio)
                .Select(c => new SesionCalendarioDto
                {
                    Id = c.Id,
                    IdMateriaPorGrupo = c.IdMateriaPorGrupo,
                    IdAula = c.IdAula,
                    AulaCodigo = c.IdAulaNavigation.Codigo,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    Activo = c.Activo,
                    EstadoAsistencia = c.Asistencium == null ? "PENDIENTE" : "REGISTRADA"
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("inscripciones-grupo")]
        public async Task<ActionResult<List<InscripcionGrupoDto>>> ListarInscripcionesPorCurso([FromQuery] Guid idMateriaPorGrupo, [FromQuery] Guid? idEstudiante, [FromQuery] bool? activo)
        {
            var query = _context.InscripcionGrupos
                .Include(ig => ig.IdEstudianteNavigation)
                .Where(ig => ig.IdMateriaPorGrupo == idMateriaPorGrupo)
                .AsNoTracking()
                .AsQueryable();

            if (idEstudiante.HasValue)
            {
                query = query.Where(ig => ig.IdEstudiante == idEstudiante.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(ig => ig.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(ig => ig.IdEstudianteNavigation.PrimerApellido)
                .ThenBy(ig => ig.IdEstudianteNavigation.Nombres)
                .Select(ig => new InscripcionGrupoDto
                {
                    Id = ig.Id,
                    IdMateriaPorGrupo = ig.IdMateriaPorGrupo,
                    IdEstudiante = ig.IdEstudiante,
                    CodigoUnico = ig.IdEstudianteNavigation.CodigoUnico,
                    Carnet = ig.IdEstudianteNavigation.Carnet,
                    Nombres = ig.IdEstudianteNavigation.Nombres,
                    PrimerApellido = ig.IdEstudianteNavigation.PrimerApellido,
                    SegundoApellido = ig.IdEstudianteNavigation.SegundoApellido,
                    FechaInscripcion = ig.FechaInscripcion,
                    Activo = ig.Activo
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
