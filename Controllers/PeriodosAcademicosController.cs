using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class PeriodosAcademicosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeriodosAcademicosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("periodos-academicos")]
        public async Task<ActionResult<List<PeriodoAcademicoDto>>> ListarPeriodosAcademicos([FromQuery] Guid? idTipoPeriodo, [FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.PeriodoAcademicos
                .Include(p => p.IdTipoPeriodoAcademicoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idTipoPeriodo.HasValue)
            {
                query = query.Where(p => p.IdTipoPeriodoAcademico == idTipoPeriodo.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p => p.Codigo.ToLower().Contains(term) || p.Nombre.ToLower().Contains(term));
            }

            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(p => p.Orden)
                .Select(p => new PeriodoAcademicoDto
                {
                    Id = p.Id,
                    IdTipoPeriodoAcademico = p.IdTipoPeriodoAcademico,
                    TipoPeriodoNombre = p.IdTipoPeriodoAcademicoNavigation != null ? p.IdTipoPeriodoAcademicoNavigation.Nombre : "",
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Orden = p.Orden,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("tipos-periodo-academico")]
        public async Task<ActionResult<List<TipoPeriodoDto>>> ListarTiposPeriodoAcademico([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.TipoPeriodoAcademicos.AsNoTracking().AsQueryable();

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
                .OrderBy(t => t.Codigo)
                .Select(t => new TipoPeriodoDto
                {
                    Id = t.Id,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    DuracionMesesMinimo = t.DuracionMesesMinimo,
                    DuracionMesesMaximo = t.DuracionMesesMaximo,
                    DuracionHorasMinimo = t.DuracionHorasMinimo,
                    DuracionHorasMaximo = t.DuracionHorasMaximo,
                    Activo = t.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("ciclos-academicos")]
        public async Task<ActionResult<List<CicloAcademicoDto>>> ListarCiclosAcademicos([FromQuery] string? search, [FromQuery] bool? activo)
        {
            var query = _context.CicloAcademicos.AsNoTracking().AsQueryable();

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
                .OrderBy(c => c.Orden)
                .Select(c => new CicloAcademicoDto
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Orden = c.Orden,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
