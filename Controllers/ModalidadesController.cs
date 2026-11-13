using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/modalidades")]
    public class ModalidadesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModalidadesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ModalidadDto>>> ListarModalidades([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.Modalidads.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(m => m.Codigo.ToLower().Contains(term) || m.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(m => m.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(m => m.Nombre)
                .Select(m => new ModalidadDto
                {
                    Id = m.Id,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Activo = m.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<ModalidadDto>> CrearModalidad([FromBody] CreateModalidadDto dto)
        {
            var exists = await _context.Modalidads.AnyAsync(m => m.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe una modalidad con este código." });
            }

            var m = new Modalidad
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Activo = dto.Activo
            };

            _context.Modalidads.Add(m);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ModalidadDto
            {
                Id = m.Id,
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                Activo = m.Activo
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ModalidadDto>> ActualizarModalidad(Guid id, [FromBody] UpdateModalidadDto dto)
        {
            var m = await _context.Modalidads.FindAsync(id);
            if (m == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Modalidad no encontrada." });
            }

            var duplicate = await _context.Modalidads.AnyAsync(x => x.Codigo == dto.Codigo.Trim() && x.Id != id);
            if (duplicate)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe otra modalidad con este código." });
            }

            m.Codigo = dto.Codigo.Trim();
            m.Nombre = dto.Nombre.Trim();
            m.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new ModalidadDto
            {
                Id = m.Id,
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                Activo = m.Activo
            });
        }
    }
}
