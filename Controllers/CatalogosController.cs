using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CatalogosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CatalogosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("centros-universitarios")]
        public async Task<ActionResult<List<CentroUniversitarioDto>>> ListarCentros([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.CentroUniversitarios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c => c.Codigo.ToLower().Contains(term) || c.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(c => c.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(c => c.Nombre)
                .Select(c => new CentroUniversitarioDto
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Ubicacion = c.Ubicacion,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("dias")]
        public async Task<ActionResult<List<DiaDto>>> ListarDias()
        {
            var items = await _context.Dia
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .Select(d => new DiaDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("tipos-evento")]
        public async Task<ActionResult<List<TipoEventoDto>>> ListarTiposEvento([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.TipoEventos.AsNoTracking().AsQueryable();

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
                .Select(t => new TipoEventoDto
                {
                    Id = t.Id,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Activo = t.Activo
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
