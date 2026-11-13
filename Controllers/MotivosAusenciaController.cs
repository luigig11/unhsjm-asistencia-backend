using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Catalogos;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/motivos-ausencia")]
    public class MotivosAusenciaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotivosAusenciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MotivoAusenciaDto>>> ListarMotivosAusencia([FromQuery] string? aplicaA, [FromQuery] bool? activo)
        {
            var query = _context.MotivoAusencia.AsNoTracking().AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(m => m.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(m => m.Nombre)
                .Select(m => new MotivoAusenciaDto
                {
                    Id = m.Id,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    JustificaInasistencia = m.JustificaInasistencia,
                    AplicaA = "AMBOS",
                    Activo = m.Activo
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(aplicaA))
            {
                items = items.Where(m => m.AplicaA == "AMBOS" || string.Equals(m.AplicaA, aplicaA, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<MotivoAusenciaDto>> CrearMotivoAusencia([FromBody] CreateMotivoAusenciaDto dto)
        {
            var exists = await _context.MotivoAusencia.AnyAsync(m => m.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe un motivo de ausencia con este código." });
            }

            var m = new MotivoAusencium
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                JustificaInasistencia = dto.JustificaInasistencia,
                AplicaA = dto.AplicaA.Trim().ToUpper(),
                Activo = dto.Activo
            };

            _context.MotivoAusencia.Add(m);
            await _context.SaveChangesAsync();

            return StatusCode(201, new MotivoAusenciaDto
            {
                Id = m.Id,
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                JustificaInasistencia = m.JustificaInasistencia,
                AplicaA = m.AplicaA,
                Activo = m.Activo
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MotivoAusenciaDto>> ActualizarMotivoAusencia(Guid id, [FromBody] UpdateMotivoAusenciaDto dto)
        {
            var m = await _context.MotivoAusencia.FindAsync(id);
            if (m == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Motivo de ausencia no encontrado." });
            }

            var duplicate = await _context.MotivoAusencia.AnyAsync(x => x.Codigo == dto.Codigo.Trim() && x.Id != id);
            if (duplicate)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe otro motivo de ausencia con este código." });
            }

            m.Codigo = dto.Codigo.Trim();
            m.Nombre = dto.Nombre.Trim();
            m.Descripcion = dto.Descripcion?.Trim();
            m.JustificaInasistencia = dto.JustificaInasistencia;
            m.AplicaA = dto.AplicaA.Trim().ToUpper();
            m.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new MotivoAusenciaDto
            {
                Id = m.Id,
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                JustificaInasistencia = m.JustificaInasistencia,
                AplicaA = m.AplicaA,
                Activo = m.Activo
            });
        }
    }
}
