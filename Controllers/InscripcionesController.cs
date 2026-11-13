using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Calendario;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/inscripciones-grupo")]
    public class InscripcionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InscripcionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<ActionResult<InscripcionGrupoDto>> CrearInscripcionGrupo([FromBody] CreateInscripcionGrupoDto dto)
        {
            var mpg = await _context.MateriaPorGrupos.FindAsync(dto.IdMateriaPorGrupo);
            if (mpg == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La materia por grupo especificada no existe." });
            }

            var estudiante = await _context.Estudiantes.FindAsync(dto.IdEstudiante);
            if (estudiante == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El estudiante especificado no existe." });
            }

            var existing = await _context.InscripcionGrupos
                .FirstOrDefaultAsync(ig => ig.IdMateriaPorGrupo == dto.IdMateriaPorGrupo && ig.IdEstudiante == dto.IdEstudiante);

            if (existing != null)
            {
                if (existing.Activo)
                {
                    return Conflict(new { status = 409, error = "CONFLICT", message = "El estudiante ya se encuentra matriculado en este grupo." });
                }

                existing.Activo = true;
                existing.FechaInscripcion = dto.FechaInscripcion ?? DateOnly.FromDateTime(DateTime.UtcNow);
                await _context.SaveChangesAsync();

                return StatusCode(201, new InscripcionGrupoDto
                {
                    Id = existing.Id,
                    IdMateriaPorGrupo = existing.IdMateriaPorGrupo,
                    IdEstudiante = existing.IdEstudiante,
                    CodigoUnico = estudiante.CodigoUnico,
                    Carnet = estudiante.Carnet,
                    Nombres = estudiante.Nombres,
                    PrimerApellido = estudiante.PrimerApellido,
                    SegundoApellido = estudiante.SegundoApellido,
                    FechaInscripcion = existing.FechaInscripcion,
                    Activo = existing.Activo
                });
            }

            var currentCount = await _context.InscripcionGrupos
                .CountAsync(ig => ig.IdMateriaPorGrupo == dto.IdMateriaPorGrupo && ig.Activo);

            if (currentCount >= mpg.CupoLimite)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "El grupo ha alcanzado el cupo límite de estudiantes." });
            }

            var inscripcion = new InscripcionGrupo
            {
                Id = Guid.NewGuid(),
                IdMateriaPorGrupo = dto.IdMateriaPorGrupo,
                IdEstudiante = dto.IdEstudiante,
                FechaInscripcion = dto.FechaInscripcion ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Activo = dto.Activo
            };

            _context.InscripcionGrupos.Add(inscripcion);
            await _context.SaveChangesAsync();

            return StatusCode(201, new InscripcionGrupoDto
            {
                Id = inscripcion.Id,
                IdMateriaPorGrupo = inscripcion.IdMateriaPorGrupo,
                IdEstudiante = inscripcion.IdEstudiante,
                CodigoUnico = estudiante.CodigoUnico,
                Carnet = estudiante.Carnet,
                Nombres = estudiante.Nombres,
                PrimerApellido = estudiante.PrimerApellido,
                SegundoApellido = estudiante.SegundoApellido,
                FechaInscripcion = inscripcion.FechaInscripcion,
                Activo = inscripcion.Activo
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarInscripcionGrupo(Guid id)
        {
            var ig = await _context.InscripcionGrupos.FindAsync(id);
            if (ig == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Inscripción no encontrada." });
            }

            _context.InscripcionGrupos.Remove(ig);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
