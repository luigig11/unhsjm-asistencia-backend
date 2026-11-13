using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Docentes;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/docentes")]
    public class DocentesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocentesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocenteItemDto>>> ListarDocentes([FromQuery] DocenteFilterDto filtro)
        {
            var query = _context.Docentes
                .Include(d => d.IdUsuarioNavigation)
                .Include(d => d.IdDireccionNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                var term = filtro.Search.Trim().ToLower();
                query = query.Where(d =>
                    d.IdUsuarioNavigation.Nombre.ToLower().Contains(term) ||
                    d.IdUsuarioNavigation.Apellido.ToLower().Contains(term) ||
                    d.CodigoEmpleado.ToLower().Contains(term) ||
                    (d.IdUsuarioNavigation.Cedula != null && d.IdUsuarioNavigation.Cedula.ToLower().Contains(term)));
            }

            if (filtro.IdDireccion.HasValue)
            {
                query = query.Where(d => d.IdDireccion == filtro.IdDireccion.Value);
            }

            if (filtro.Activo.HasValue)
            {
                query = query.Where(d => d.Activo == filtro.Activo.Value);
            }

            var docentes = await query
                .OrderBy(d => d.IdUsuarioNavigation.Apellido)
                .ThenBy(d => d.IdUsuarioNavigation.Nombre)
                .Select(d => new DocenteItemDto
                {
                    Id = d.Id,
                    IdUsuario = d.IdUsuario,
                    NombreCompleto = $"{d.IdUsuarioNavigation.Nombre} {d.IdUsuarioNavigation.Apellido}",
                    Cedula = d.IdUsuarioNavigation.Cedula,
                    IdDireccion = d.IdDireccion,
                    DireccionNombre = d.IdDireccionNavigation.Nombre,
                    CodigoEmpleado = d.CodigoEmpleado,
                    Telefono = d.Telefono,
                    TipoContratacion = "FIJO",
                    Activo = d.Activo
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(filtro.TipoContratacion))
            {
                docentes = docentes.Where(d => string.Equals(d.TipoContratacion, filtro.TipoContratacion, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(docentes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DocenteDetalleDto>> ObtenerDocentePorId(Guid id)
        {
            var d = await _context.Docentes
                .Include(doc => doc.IdUsuarioNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(doc => doc.Id == id);

            if (d == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Docente con ID {id} no encontrado."
                });
            }

            return Ok(new DocenteDetalleDto
            {
                Id = d.Id,
                IdUsuario = d.IdUsuario,
                Usuario = new DocenteUsuarioResumenDto
                {
                    Nombre = d.IdUsuarioNavigation.Nombre,
                    Apellido = d.IdUsuarioNavigation.Apellido,
                    Cedula = d.IdUsuarioNavigation.Cedula,
                    Genero = "M"
                },
                IdDireccion = d.IdDireccion,
                CodigoEmpleado = d.CodigoEmpleado,
                Telefono = d.Telefono,
                TipoContratacion = "FIJO",
                Activo = d.Activo
            });
        }

        [HttpPost]
        public async Task<ActionResult<DocenteDetalleDto>> CrearDocente([FromBody] CreateDocenteDto dto)
        {
            var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Id == dto.IdUsuario);
            if (!existeUsuario)
            {
                return BadRequest(new
                {
                    status = 400,
                    error = "BAD_REQUEST",
                    message = "El usuario especificado no existe."
                });
            }

            var usuarioYaDocente = await _context.Docentes.AnyAsync(d => d.IdUsuario == dto.IdUsuario);
            if (usuarioYaDocente)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "El usuario ya cuenta con un expediente docente registrado."
                });
            }

            var existeCodigo = await _context.Docentes.AnyAsync(d => d.CodigoEmpleado == dto.CodigoEmpleado.Trim());
            if (existeCodigo)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe un docente con este Código de Empleado."
                });
            }

            var docente = new Docente
            {
                Id = Guid.NewGuid(),
                IdUsuario = dto.IdUsuario,
                IdDireccion = dto.IdDireccion,
                CodigoEmpleado = dto.CodigoEmpleado.Trim(),
                Telefono = dto.Telefono?.Trim(),
                Activo = dto.Activo
            };

            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            var u = await _context.Usuarios.FirstAsync(user => user.Id == dto.IdUsuario);

            return CreatedAtAction(nameof(ObtenerDocentePorId), new { id = docente.Id }, new DocenteDetalleDto
            {
                Id = docente.Id,
                IdUsuario = docente.IdUsuario,
                Usuario = new DocenteUsuarioResumenDto
                {
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Cedula = u.Cedula,
                    Genero = "M"
                },
                IdDireccion = docente.IdDireccion,
                CodigoEmpleado = docente.CodigoEmpleado,
                Telefono = docente.Telefono,
                TipoContratacion = dto.TipoContratacion,
                Activo = docente.Activo
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DocenteDetalleDto>> ActualizarDocente(Guid id, [FromBody] UpdateDocenteDto dto)
        {
            var docente = await _context.Docentes
                .Include(d => d.IdUsuarioNavigation)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Docente con ID {id} no encontrado."
                });
            }

            var existeCodigo = await _context.Docentes.AnyAsync(d => d.CodigoEmpleado == dto.CodigoEmpleado.Trim() && d.Id != id);
            if (existeCodigo)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe otro docente con este Código de Empleado."
                });
            }

            docente.IdDireccion = dto.IdDireccion;
            docente.CodigoEmpleado = dto.CodigoEmpleado.Trim();
            docente.Telefono = dto.Telefono?.Trim();
            docente.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new DocenteDetalleDto
            {
                Id = docente.Id,
                IdUsuario = docente.IdUsuario,
                Usuario = new DocenteUsuarioResumenDto
                {
                    Nombre = docente.IdUsuarioNavigation.Nombre,
                    Apellido = docente.IdUsuarioNavigation.Apellido,
                    Cedula = docente.IdUsuarioNavigation.Cedula,
                    Genero = "M"
                },
                IdDireccion = docente.IdDireccion,
                CodigoEmpleado = docente.CodigoEmpleado,
                Telefono = docente.Telefono,
                TipoContratacion = dto.TipoContratacion,
                Activo = docente.Activo
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> EliminarDocente(Guid id)
        {
            var docente = await _context.Docentes
                .Include(d => d.MateriaPorGrupos)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Docente con ID {id} no encontrado."
                });
            }

            if (docente.MateriaPorGrupos.Any())
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "No es posible eliminar el docente porque tiene materias asignadas."
                });
            }

            _context.Docentes.Remove(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
