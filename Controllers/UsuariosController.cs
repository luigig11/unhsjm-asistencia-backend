using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedUsuariosDto>> ListarUsuarios([FromQuery] UsuarioFilterDto filtro)
        {
            var query = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.Docente)
                .Include(u => u.AuthUser)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                var term = filtro.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(term) ||
                    u.Apellido.ToLower().Contains(term) ||
                    (u.Cedula != null && u.Cedula.ToLower().Contains(term)) ||
                    (u.AuthUser != null && u.AuthUser.Email != null && u.AuthUser.Email.ToLower().Contains(term))
                );
            }

            if (filtro.IdRol.HasValue)
            {
                query = query.Where(u => u.IdRol == filtro.IdRol.Value);
            }

            if (filtro.Activo.HasValue)
            {
                query = query.Where(u => u.Activo == filtro.Activo.Value);
            }

            var total = await query.CountAsync();

            var page = filtro.Page > 0 ? filtro.Page : 1;
            var pageSize = filtro.PageSize > 0 ? filtro.PageSize : 20;

            var items = await query
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UsuarioListItemDto
                {
                    Id = u.Id,
                    IdUser = u.IdUser,
                    IdRol = u.IdRol,
                    RolNombre = u.IdRolNavigation.Nombre,
                    Cedula = u.Cedula,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Genero = "M",
                    FechaNacimiento = u.FechaNacimiento.HasValue ? u.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
                    Activo = u.Activo,
                    TienePerfilDocente = u.Docente != null,
                    IdDocente = u.Docente != null ? u.Docente.Id : null
                })
                .ToListAsync();

            return Ok(new PagedUsuariosDto
            {
                Total = total,
                Items = items
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> ObtenerUsuarioPorId(Guid id)
        {
            var u = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.Docente)
                .Include(u => u.AuthUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (u == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Usuario no encontrado." });
            }

            return Ok(new
            {
                id = u.Id,
                id_user = u.IdUser,
                id_rol = u.IdRol,
                rol_nombre = u.IdRolNavigation.Nombre,
                cedula = u.Cedula,
                nombre = u.Nombre,
                apellido = u.Apellido,
                genero = "M",
                fecha_nacimiento = u.FechaNacimiento.HasValue ? u.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
                activo = u.Activo,
                docente = u.Docente != null ? new
                {
                    id = u.Docente.Id,
                    id_direccion = u.Docente.IdDireccion,
                    codigo_empleado = u.Docente.CodigoEmpleado,
                    telefono = u.Docente.Telefono,
                    tipo_contratacion = "FIJO",
                    activo = u.Docente.Activo
                } : null
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> ActualizarUsuario(Guid id, [FromBody] UpdateUsuarioDto dto)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Usuario no encontrado." });
            }

            var rolExists = await _context.Rols.AnyAsync(r => r.Id == dto.IdRol);
            if (!rolExists)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El rol especificado no existe." });
            }

            user.IdRol = dto.IdRol;
            user.Cedula = dto.Cedula?.Trim();
            user.Nombre = dto.Nombre.Trim();
            user.Apellido = dto.Apellido.Trim();
            user.FechaNacimiento = dto.FechaNacimiento;
            user.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = user.Id,
                id_user = user.IdUser,
                id_rol = user.IdRol,
                cedula = user.Cedula,
                nombre = user.Nombre,
                apellido = user.Apellido,
                genero = dto.Genero,
                fecha_nacimiento = user.FechaNacimiento.HasValue ? user.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
                activo = user.Activo
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> EliminarUsuario(Guid id)
        {
            var user = await _context.Usuarios
                .Include(u => u.Docente)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = "Usuario no encontrado." });
            }

            if (user.Docente != null)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "No se puede eliminar un usuario con perfil de docente asociado." });
            }

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
