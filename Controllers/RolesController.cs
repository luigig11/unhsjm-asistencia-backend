using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RolDto>>> ListarRoles([FromQuery] bool? activo)
        {
            var query = _context.Rols.AsNoTracking().AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(r => r.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(r => r.Nombre)
                .Select(r => new RolDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Nombre = r.Nombre,
                    Activo = r.Activo
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
