using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class AulasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AulasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("aulas")]
        public async Task<ActionResult<List<AulaDto>>> ListarAulas([FromQuery] Guid? idTipoAula, [FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.Aulas
                .Include(a => a.IdTipoAulaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idTipoAula.HasValue)
            {
                query = query.Where(a => a.IdTipoAula == idTipoAula.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(a => a.Codigo.ToLower().Contains(term) || a.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(a => a.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(a => a.Codigo)
                .Select(a => new AulaDto
                {
                    Id = a.Id,
                    IdTipoAula = a.IdTipoAula,
                    TipoAulaNombre = a.IdTipoAulaNavigation.Nombre,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Capacidad = a.Capacidad,
                    Ubicacion = a.Ubicacion,
                    Activo = a.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("aulas")]
        public async Task<ActionResult<AulaDto>> CrearAula([FromBody] CreateAulaDto dto)
        {
            var exists = await _context.Aulas.AnyAsync(a => a.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe un aula con este código." });
            }

            var tipoAula = await _context.TipoAulas.FindAsync(dto.IdTipoAula);
            if (tipoAula == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El tipo de aula especificado no existe." });
            }

            var aula = new Aula
            {
                Id = Guid.NewGuid(),
                IdTipoAula = dto.IdTipoAula,
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Capacidad = dto.Capacidad,
                Ubicacion = dto.Ubicacion?.Trim(),
                Activo = dto.Activo
            };

            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            return StatusCode(201, new AulaDto
            {
                Id = aula.Id,
                IdTipoAula = aula.IdTipoAula,
                TipoAulaNombre = tipoAula.Nombre,
                Codigo = aula.Codigo,
                Nombre = aula.Nombre,
                Capacidad = aula.Capacidad,
                Ubicacion = aula.Ubicacion,
                Activo = aula.Activo
            });
        }

        [HttpPut("aulas/{id}")]
        public async Task<ActionResult<AulaDto>> ActualizarAula(Guid id, [FromBody] UpdateAulaDto dto)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Aula no encontrada." });
            }

            var duplicate = await _context.Aulas.AnyAsync(a => a.Codigo == dto.Codigo.Trim() && a.Id != id);
            if (duplicate)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe otra aula con este código." });
            }

            var tipoAula = await _context.TipoAulas.FindAsync(dto.IdTipoAula);
            if (tipoAula == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El tipo de aula especificado no existe." });
            }

            aula.IdTipoAula = dto.IdTipoAula;
            aula.Codigo = dto.Codigo.Trim();
            aula.Nombre = dto.Nombre.Trim();
            aula.Capacidad = dto.Capacidad;
            aula.Ubicacion = dto.Ubicacion?.Trim();
            aula.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new AulaDto
            {
                Id = aula.Id,
                IdTipoAula = aula.IdTipoAula,
                TipoAulaNombre = tipoAula.Nombre,
                Codigo = aula.Codigo,
                Nombre = aula.Nombre,
                Capacidad = aula.Capacidad,
                Ubicacion = aula.Ubicacion,
                Activo = aula.Activo
            });
        }

        [HttpGet("tipos-aula")]
        public async Task<ActionResult<List<TipoAulaDto>>> ListarTiposAula([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.TipoAulas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(t => t.Codigo.ToLower().Contains(term) || t.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(t => t.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(t => t.Nombre)
                .Select(t => new TipoAulaDto
                {
                    Id = t.Id,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Activo = t.Activo
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
