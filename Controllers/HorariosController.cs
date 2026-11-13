using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Calendario;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/calendarios-semanales")]
    public class HorariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HorariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CalendarioSemanalDto>>> ListarCalendarioSemanal([FromQuery] Guid? idMateriaPorGrupo, [FromQuery] bool? activo)
        {
            var query = _context.CalendarioSemanals
                .Include(cs => cs.IdAulaNavigation)
                .Include(cs => cs.IdDiaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idMateriaPorGrupo.HasValue)
            {
                query = query.Where(cs => cs.IdMateriaPorGrupo == idMateriaPorGrupo.Value);
            }

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

        [HttpPost]
        public async Task<ActionResult<CalendarioSemanalDto>> CrearCalendarioSemanal([FromBody] CreateCalendarioSemanalDto dto)
        {
            if (dto.HoraFin <= dto.HoraInicio)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La hora de fin debe ser posterior a la hora de inicio." });
            }

            var mpg = await _context.MateriaPorGrupos.FindAsync(dto.IdMateriaPorGrupo);
            if (mpg == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La materia por grupo especificada no existe." });
            }

            var aula = await _context.Aulas.FindAsync(dto.IdAula);
            if (aula == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El aula especificada no existe." });
            }

            var dia = await _context.Dia.FindAsync(dto.IdDia);
            if (dia == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El día especificado no existe." });
            }

            var existing = await _context.CalendarioSemanals
                .Where(cs => cs.IdAula == dto.IdAula && cs.IdDia == dto.IdDia && cs.Activo)
                .ToListAsync();

            var overlap = existing.Any(cs => dto.HoraInicio < cs.HoraFin && dto.HoraFin > cs.HoraInicio);
            if (overlap)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "El aula seleccionada ya tiene un horario asignado que se solapa con el intervalo solicitado." });
            }

            var csEntity = new CalendarioSemanal
            {
                Id = Guid.NewGuid(),
                IdMateriaPorGrupo = dto.IdMateriaPorGrupo,
                IdAula = dto.IdAula,
                IdDia = dto.IdDia,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Activo = dto.Activo
            };

            _context.CalendarioSemanals.Add(csEntity);
            await _context.SaveChangesAsync();

            return StatusCode(201, new CalendarioSemanalDto
            {
                Id = csEntity.Id,
                IdMateriaPorGrupo = csEntity.IdMateriaPorGrupo,
                IdAula = csEntity.IdAula,
                AulaCodigo = aula.Codigo,
                AulaNombre = aula.Nombre,
                IdDia = csEntity.IdDia,
                DiaNombre = dia.Nombre,
                HoraInicio = csEntity.HoraInicio,
                HoraFin = csEntity.HoraFin,
                Activo = csEntity.Activo
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CalendarioSemanalDto>> ActualizarCalendarioSemanal(Guid id, [FromBody] UpdateCalendarioSemanalDto dto)
        {
            if (dto.HoraFin <= dto.HoraInicio)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La hora de fin debe ser posterior a la hora de inicio." });
            }

            var csEntity = await _context.CalendarioSemanals.FindAsync(id);
            if (csEntity == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Bloque de horario no encontrado." });
            }

            var aula = await _context.Aulas.FindAsync(dto.IdAula);
            if (aula == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El aula especificada no existe." });
            }

            var dia = await _context.Dia.FindAsync(dto.IdDia);
            if (dia == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El día especificado no existe." });
            }

            var existing = await _context.CalendarioSemanals
                .Where(cs => cs.Id != id && cs.IdAula == dto.IdAula && cs.IdDia == dto.IdDia && cs.Activo)
                .ToListAsync();

            var overlap = existing.Any(cs => dto.HoraInicio < cs.HoraFin && dto.HoraFin > cs.HoraInicio);
            if (overlap)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "El aula seleccionada ya tiene un horario asignado que se solapa con el intervalo solicitado." });
            }

            if (dto.IdMateriaPorGrupo.HasValue && dto.IdMateriaPorGrupo.Value != Guid.Empty)
            {
                csEntity.IdMateriaPorGrupo = dto.IdMateriaPorGrupo.Value;
            }

            csEntity.IdAula = dto.IdAula;
            csEntity.IdDia = dto.IdDia;
            csEntity.HoraInicio = dto.HoraInicio;
            csEntity.HoraFin = dto.HoraFin;
            csEntity.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new CalendarioSemanalDto
            {
                Id = csEntity.Id,
                IdMateriaPorGrupo = csEntity.IdMateriaPorGrupo,
                IdAula = csEntity.IdAula,
                AulaCodigo = aula.Codigo,
                AulaNombre = aula.Nombre,
                IdDia = csEntity.IdDia,
                DiaNombre = dia.Nombre,
                HoraInicio = csEntity.HoraInicio,
                HoraFin = csEntity.HoraFin,
                Activo = csEntity.Activo
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCalendarioSemanal(Guid id)
        {
            var cs = await _context.CalendarioSemanals.FindAsync(id);
            if (cs == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Bloque de horario no encontrado." });
            }

            _context.CalendarioSemanals.Remove(cs);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
