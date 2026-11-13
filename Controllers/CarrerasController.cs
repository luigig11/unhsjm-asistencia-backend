using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/carreras")]
    public class CarrerasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarrerasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarreraDto>>> ListarCarreras([FromQuery] string? search, [FromQuery] Guid? idDireccion, [FromQuery] bool? activo)
        {
            var query = _context.Carreras
                .Include(c => c.IdDireccionNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c => c.Codigo.ToLower().Contains(term) || c.Nombre.ToLower().Contains(term));
            }

            if (idDireccion.HasValue)
            {
                query = query.Where(c => c.IdDireccion == idDireccion.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(c => c.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(c => c.Nombre)
                .Select(c => new CarreraDto
                {
                    Id = c.Id,
                    IdDireccion = c.IdDireccion,
                    DireccionNombre = c.IdDireccionNavigation.Nombre,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<CarreraDto>> CrearCarrera([FromBody] CreateCarreraDto dto)
        {
            var exists = await _context.Carreras.AnyAsync(c => c.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe una carrera con este código." });
            }

            var c = new Carrera
            {
                Id = Guid.NewGuid(),
                IdDireccion = dto.IdDireccion,
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Activo = dto.Activo
            };

            _context.Carreras.Add(c);
            await _context.SaveChangesAsync();

            var dir = await _context.Direccions.FindAsync(dto.IdDireccion);

            return StatusCode(201, new CarreraDto
            {
                Id = c.Id,
                IdDireccion = c.IdDireccion,
                DireccionNombre = dir?.Nombre ?? "",
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                Activo = c.Activo
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarreraDto>> ActualizarCarrera(Guid id, [FromBody] UpdateCarreraDto dto)
        {
            var carrera = await _context.Carreras.Include(c => c.IdDireccionNavigation).FirstOrDefaultAsync(c => c.Id == id);
            if (carrera == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Carrera no encontrada." });
            }

            var duplicate = await _context.Carreras.AnyAsync(c => c.Codigo == dto.Codigo.Trim() && c.Id != id);
            if (duplicate)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe otra carrera con este código." });
            }

            carrera.IdDireccion = dto.IdDireccion;
            carrera.Codigo = dto.Codigo.Trim();
            carrera.Nombre = dto.Nombre.Trim();
            carrera.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            var dir = await _context.Direccions.FindAsync(dto.IdDireccion);

            return Ok(new CarreraDto
            {
                Id = carrera.Id,
                IdDireccion = carrera.IdDireccion,
                DireccionNombre = dir?.Nombre ?? "",
                Codigo = carrera.Codigo,
                Nombre = carrera.Nombre,
                Activo = carrera.Activo
            });
        }
    }
}
