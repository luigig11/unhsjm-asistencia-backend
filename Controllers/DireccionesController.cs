using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/direcciones")]
    public class DireccionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DireccionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<DireccionDto>>> ListarDirecciones([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.Direccions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(d => d.Codigo.ToLower().Contains(term) || d.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(d => d.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(d => d.Nombre)
                .Select(d => new DireccionDto
                {
                    Id = d.Id,
                    Codigo = d.Codigo,
                    Nombre = d.Nombre,
                    Activo = d.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<DireccionDto>> CrearDireccion([FromBody] CreateDireccionDto dto)
        {
            var exists = await _context.Direccions.AnyAsync(d => d.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe una dirección con este código." });
            }

            var dir = new Direccion
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Activo = dto.Activo
            };

            _context.Direccions.Add(dir);
            await _context.SaveChangesAsync();

            return StatusCode(201, new DireccionDto
            {
                Id = dir.Id,
                Codigo = dir.Codigo,
                Nombre = dir.Nombre,
                Activo = dir.Activo
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DireccionDto>> ActualizarDireccion(Guid id, [FromBody] UpdateDireccionDto dto)
        {
            var dir = await _context.Direccions.FindAsync(id);
            if (dir == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Dirección no encontrada." });
            }

            var duplicate = await _context.Direccions.AnyAsync(d => d.Codigo == dto.Codigo.Trim() && d.Id != id);
            if (duplicate)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe otra dirección con este código." });
            }

            dir.Codigo = dto.Codigo.Trim();
            dir.Nombre = dto.Nombre.Trim();
            dir.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new DireccionDto
            {
                Id = dir.Id,
                Codigo = dir.Codigo,
                Nombre = dir.Nombre,
                Activo = dir.Activo
            });
        }
    }
}
