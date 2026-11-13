using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.DTOs.Estudiantes;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/estudiantes")]
    public class EstudiantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstudiantesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<EstudianteItemDto>>> ListarEstudiantes([FromQuery] EstudianteFilterDto filtro)
        {
            var query = _context.Estudiantes
                .Include(e => e.IdCarreraNavigation)
                .Include(e => e.IdPlanAcademicoNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                var term = filtro.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.Nombres.ToLower().Contains(term) ||
                    e.PrimerApellido.ToLower().Contains(term) ||
                    (e.SegundoApellido != null && e.SegundoApellido.ToLower().Contains(term)) ||
                    e.CodigoUnico.ToLower().Contains(term) ||
                    e.Carnet.ToLower().Contains(term) ||
                    (e.Cedula != null && e.Cedula.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoUnico))
            {
                query = query.Where(e => e.CodigoUnico == filtro.CodigoUnico.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.Carnet))
            {
                query = query.Where(e => e.Carnet == filtro.Carnet.Trim());
            }

            if (filtro.IdCarrera.HasValue)
            {
                query = query.Where(e => e.IdCarrera == filtro.IdCarrera.Value);
            }

            if (filtro.IdPlanAcademico.HasValue)
            {
                query = query.Where(e => e.IdPlanAcademico == filtro.IdPlanAcademico.Value);
            }

            if (filtro.Activo.HasValue)
            {
                query = query.Where(e => e.Activo == filtro.Activo.Value);
            }

            var total = await query.CountAsync();

            var page = filtro.Page <= 0 ? 1 : filtro.Page;
            var pageSize = filtro.PageSize <= 0 ? 20 : filtro.PageSize;

            var items = await query
                .OrderBy(e => e.PrimerApellido)
                .ThenBy(e => e.Nombres)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EstudianteItemDto
                {
                    Id = e.Id,
                    CodigoUnico = e.CodigoUnico,
                    Carnet = e.Carnet,
                    Cedula = e.Cedula,
                    Nombres = e.Nombres,
                    PrimerApellido = e.PrimerApellido,
                    SegundoApellido = e.SegundoApellido,
                    Correo = e.Correo,
                    Telefono = e.Telefono,
                    FechaIngreso = e.FechaIngreso,
                    IdCarrera = e.IdCarrera,
                    CarreraNombre = e.IdCarreraNavigation.Nombre,
                    IdPlanAcademico = e.IdPlanAcademico,
                    PlanCodigo = e.IdPlanAcademicoNavigation.Codigo,
                    Activo = e.Activo
                })
                .ToListAsync();

            return Ok(new PagedResultDto<EstudianteItemDto>
            {
                Total = total,
                Items = items
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EstudianteDetalleDto>> ObtenerEstudiantePorId(Guid id)
        {
            var estudiante = await _context.Estudiantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Estudiante con ID {id} no encontrado."
                });
            }

            return Ok(new EstudianteDetalleDto
            {
                Id = estudiante.Id,
                CodigoUnico = estudiante.CodigoUnico,
                IdCarrera = estudiante.IdCarrera,
                IdPlanAcademico = estudiante.IdPlanAcademico,
                Carnet = estudiante.Carnet,
                Cedula = estudiante.Cedula,
                Nombres = estudiante.Nombres,
                PrimerApellido = estudiante.PrimerApellido,
                SegundoApellido = estudiante.SegundoApellido,
                Correo = estudiante.Correo,
                Telefono = estudiante.Telefono,
                FechaIngreso = estudiante.FechaIngreso,
                Activo = estudiante.Activo
            });
        }

        [HttpPost]
        public async Task<ActionResult<EstudianteDetalleDto>> CrearEstudiante([FromBody] CreateEstudianteDto dto)
        {
            var codigoTrimmed = dto.CodigoUnico.Trim();

            var existeCodigo = await _context.Estudiantes.AnyAsync(e => e.CodigoUnico == codigoTrimmed);
            if (existeCodigo)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe un estudiante con este Código Único."
                });
            }

            var existeCarnet = await _context.Estudiantes.AnyAsync(e => e.Carnet == dto.Carnet.Trim());
            if (existeCarnet)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe un estudiante con este Carnet."
                });
            }

            if (!string.IsNullOrWhiteSpace(dto.Cedula))
            {
                var existeCedula = await _context.Estudiantes.AnyAsync(e => e.Cedula == dto.Cedula.Trim());
                if (existeCedula)
                {
                    return Conflict(new
                    {
                        status = 409,
                        error = "CONFLICT",
                        message = "Ya existe un estudiante con esta Cédula."
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Correo))
            {
                var existeCorreo = await _context.Estudiantes.AnyAsync(e => e.Correo == dto.Correo.Trim());
                if (existeCorreo)
                {
                    return Conflict(new
                    {
                        status = 409,
                        error = "CONFLICT",
                        message = "Ya existe un estudiante con este Correo Electrónico."
                    });
                }
            }

            var estudiante = new Estudiante
            {
                Id = Guid.NewGuid(),
                CodigoUnico = codigoTrimmed,
                IdCarrera = dto.IdCarrera,
                IdPlanAcademico = dto.IdPlanAcademico,
                Carnet = dto.Carnet.Trim(),
                Cedula = dto.Cedula?.Trim(),
                Nombres = dto.Nombres.Trim(),
                PrimerApellido = dto.PrimerApellido.Trim(),
                SegundoApellido = dto.SegundoApellido?.Trim(),
                Correo = dto.Correo?.Trim(),
                Telefono = dto.Telefono?.Trim(),
                FechaIngreso = dto.FechaIngreso,
                Activo = dto.Activo
            };

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerEstudiantePorId), new { id = estudiante.Id }, new EstudianteDetalleDto
            {
                Id = estudiante.Id,
                CodigoUnico = estudiante.CodigoUnico,
                IdCarrera = estudiante.IdCarrera,
                IdPlanAcademico = estudiante.IdPlanAcademico,
                Carnet = estudiante.Carnet,
                Cedula = estudiante.Cedula,
                Nombres = estudiante.Nombres,
                PrimerApellido = estudiante.PrimerApellido,
                SegundoApellido = estudiante.SegundoApellido,
                Correo = estudiante.Correo,
                Telefono = estudiante.Telefono,
                FechaIngreso = estudiante.FechaIngreso,
                Activo = estudiante.Activo
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EstudianteDetalleDto>> ActualizarEstudiante(Guid id, [FromBody] UpdateEstudianteDto dto)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Estudiante con ID {id} no encontrado."
                });
            }

            var codigoTrimmed = dto.CodigoUnico.Trim();
            var existeCodigo = await _context.Estudiantes.AnyAsync(e => e.CodigoUnico == codigoTrimmed && e.Id != id);
            if (existeCodigo)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe otro estudiante con este Código Único."
                });
            }

            var existeCarnet = await _context.Estudiantes.AnyAsync(e => e.Carnet == dto.Carnet.Trim() && e.Id != id);
            if (existeCarnet)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe otro estudiante con este Carnet."
                });
            }

            if (!string.IsNullOrWhiteSpace(dto.Cedula))
            {
                var existeCedula = await _context.Estudiantes.AnyAsync(e => e.Cedula == dto.Cedula.Trim() && e.Id != id);
                if (existeCedula)
                {
                    return Conflict(new
                    {
                        status = 409,
                        error = "CONFLICT",
                        message = "Ya existe otro estudiante con esta Cédula."
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Correo))
            {
                var existeCorreo = await _context.Estudiantes.AnyAsync(e => e.Correo == dto.Correo.Trim() && e.Id != id);
                if (existeCorreo)
                {
                    return Conflict(new
                    {
                        status = 409,
                        error = "CONFLICT",
                        message = "Ya existe otro estudiante con este Correo Electrónico."
                    });
                }
            }

            estudiante.CodigoUnico = codigoTrimmed;
            estudiante.IdCarrera = dto.IdCarrera;
            estudiante.IdPlanAcademico = dto.IdPlanAcademico;
            estudiante.Carnet = dto.Carnet.Trim();
            estudiante.Cedula = dto.Cedula?.Trim();
            estudiante.Nombres = dto.Nombres.Trim();
            estudiante.PrimerApellido = dto.PrimerApellido.Trim();
            estudiante.SegundoApellido = dto.SegundoApellido?.Trim();
            estudiante.Correo = dto.Correo?.Trim();
            estudiante.Telefono = dto.Telefono?.Trim();
            estudiante.FechaIngreso = dto.FechaIngreso;
            estudiante.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new EstudianteDetalleDto
            {
                Id = estudiante.Id,
                CodigoUnico = estudiante.CodigoUnico,
                IdCarrera = estudiante.IdCarrera,
                IdPlanAcademico = estudiante.IdPlanAcademico,
                Carnet = estudiante.Carnet,
                Cedula = estudiante.Cedula,
                Nombres = estudiante.Nombres,
                PrimerApellido = estudiante.PrimerApellido,
                SegundoApellido = estudiante.SegundoApellido,
                Correo = estudiante.Correo,
                Telefono = estudiante.Telefono,
                FechaIngreso = estudiante.FechaIngreso,
                Activo = estudiante.Activo
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> EliminarEstudiante(Guid id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.InscripcionGrupos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Estudiante con ID {id} no encontrado."
                });
            }

            if (estudiante.InscripcionGrupos.Any())
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "No es posible eliminar el estudiante porque posee inscripciones académicas registradas."
                });
            }

            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
