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

        [HttpPost("periodos-escolares")]
        public async Task<ActionResult<PeriodoEscolarDto>> CrearPeriodoEscolar([FromBody] CreatePeriodoEscolarDto dto)
        {
            var exists = await _context.PeriodoEscolars.AnyAsync(p => p.NombrePeriodoEscolar == dto.NombrePeriodoEscolar.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe un período escolar con este nombre." });
            }

            if (dto.FechaFin <= dto.FechaInicio)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La fecha de fin debe ser posterior a la fecha de inicio." });
            }

            var periodo = new PeriodoEscolar
            {
                Id = Guid.NewGuid(),
                NombrePeriodoEscolar = dto.NombrePeriodoEscolar.Trim(),
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Activo = dto.Activo
            };

            _context.PeriodoEscolars.Add(periodo);
            await _context.SaveChangesAsync();

            return StatusCode(201, new PeriodoEscolarDto
            {
                Id = periodo.Id,
                NombrePeriodoEscolar = periodo.NombrePeriodoEscolar,
                FechaInicio = periodo.FechaInicio,
                FechaFin = periodo.FechaFin,
                Activo = periodo.Activo
            });
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

        [HttpPost("feriados")]
        public async Task<ActionResult<FeriadoDto>> CrearFeriado([FromBody] CreateFeriadoDto dto)
        {
            var periodo = await _context.PeriodoEscolars.FindAsync(dto.IdPeriodoEscolar);
            if (periodo == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El período escolar especificado no existe." });
            }

            var tipoEvento = await _context.TipoEventos.FindAsync(dto.IdTipoEvento);
            if (tipoEvento == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El tipo de evento especificado no existe." });
            }

            var feriado = new Feriado
            {
                Id = Guid.NewGuid(),
                IdPeriodoEscolar = dto.IdPeriodoEscolar,
                IdTipoEvento = dto.IdTipoEvento,
                Nombre = dto.Nombre.Trim(),
                FechaInicio = DateTime.SpecifyKind(dto.FechaInicio, DateTimeKind.Utc),
                FechaFin = DateTime.SpecifyKind(dto.FechaFin, DateTimeKind.Utc),
                Activo = dto.Activo
            };

            _context.Feriados.Add(feriado);
            await _context.SaveChangesAsync();

            return StatusCode(201, new FeriadoDto
            {
                Id = feriado.Id,
                IdPeriodoEscolar = feriado.IdPeriodoEscolar,
                IdTipoEvento = feriado.IdTipoEvento,
                TipoEventoCodigo = tipoEvento.Codigo,
                TipoEventoNombre = tipoEvento.Nombre,
                Nombre = feriado.Nombre,
                FechaInicio = feriado.FechaInicio,
                FechaFin = feriado.FechaFin,
                Activo = feriado.Activo
            });
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

        [HttpPost("instancias-academicas")]
        public async Task<ActionResult<InstanciaAcademicaDto>> CrearInstanciaAcademica([FromBody] CreateInstanciaDto dto)
        {
            var periodo = await _context.PeriodoEscolars.FindAsync(dto.IdPeriodoEscolar);
            if (periodo == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El período escolar especificado no existe." });
            }

            var detallePlan = await _context.DetallePlanAcademicos
                .Include(d => d.IdPlanAcademicoNavigation)
                .Include(d => d.IdCicloAcademicoNavigation)
                .Include(d => d.IdPeriodoAcademicoNavigation)
                .FirstOrDefaultAsync(d => d.Id == dto.IdDetallePlanAcademico);

            if (detallePlan == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El detalle de plan académico especificado no existe." });
            }

            var instancia = new InstanciaAcademica
            {
                Id = Guid.NewGuid(),
                IdPeriodoEscolar = dto.IdPeriodoEscolar,
                IdDetallePlanAcademico = dto.IdDetallePlanAcademico,
                Nombre = dto.Nombre.Trim(),
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Activo = dto.Activo
            };

            _context.InstanciaAcademicas.Add(instancia);
            await _context.SaveChangesAsync();

            return StatusCode(201, new InstanciaAcademicaDto
            {
                Id = instancia.Id,
                IdPeriodoEscolar = instancia.IdPeriodoEscolar,
                NombrePeriodoEscolar = periodo.NombrePeriodoEscolar,
                IdDetallePlanAcademico = instancia.IdDetallePlanAcademico,
                PlanCodigo = detallePlan.IdPlanAcademicoNavigation.Codigo,
                CicloNombre = detallePlan.IdCicloAcademicoNavigation.Nombre,
                PeriodoNombre = detallePlan.IdPeriodoAcademicoNavigation.Nombre,
                Nombre = instancia.Nombre,
                FechaInicio = instancia.FechaInicio,
                FechaFin = instancia.FechaFin,
                Activo = instancia.Activo
            });
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
                var inicio = DateTime.SpecifyKind(fecha.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                var fin = DateTime.SpecifyKind(fecha.Value.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);
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
    }
}
