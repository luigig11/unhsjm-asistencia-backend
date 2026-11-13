using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var emailInput = dto.Email.Trim().ToLower();

            
            var authUser = await _context.AuthUsers
                .FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == emailInput);

            Usuario? user = null;

            if (authUser != null)
            {
                user = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .Include(u => u.Docente)
                    .Include(u => u.AuthUser)
                    .FirstOrDefaultAsync(u => u.Activo && u.IdUser == authUser.Id);
            }

            
            if (user == null)
            {
                var alias = emailInput.Contains('@') ? emailInput.Split('@')[0] : emailInput;
                user = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .Include(u => u.Docente)
                    .Include(u => u.AuthUser)
                    .FirstOrDefaultAsync(u => u.Activo && (
                        u.Nombre.ToLower() == alias || 
                        u.Nombre.ToLower() == emailInput || 
                        (u.Cedula != null && u.Cedula == dto.Email.Trim())
                    ));

                if (user?.IdUser != null && authUser == null)
                {
                    authUser = await _context.AuthUsers.FindAsync(user.IdUser);
                }
            }

            if (user == null)
            {
                return Unauthorized(new
                {
                    status = 401,
                    error = "UNAUTHORIZED",
                    message = "Credenciales incorrectas o usuario inactivo."
                });
            }

            
            if (authUser?.EncryptedPassword != null && !string.IsNullOrWhiteSpace(dto.Password))
            {
                bool passwordMatches = false;
                try
                {
                    passwordMatches = BCrypt.Net.BCrypt.Verify(dto.Password, authUser.EncryptedPassword);
                }
                catch
                {
                    passwordMatches = (dto.Password == authUser.EncryptedPassword);
                }

                if (!passwordMatches)
                {
                    return Unauthorized(new
                    {
                        status = 401,
                        error = "UNAUTHORIZED",
                        message = "Contraseña incorrecta."
                    });
                }
            }

            var tokenFake = Guid.NewGuid().ToString("N");
            var refreshFake = Guid.NewGuid().ToString("N");

            return Ok(new AuthResponseDto
            {
                AccessToken = $"UNHSJM.{user.Id}.{tokenFake}",
                RefreshToken = refreshFake,
                ExpiresIn = 3600,
                Usuario = new UsuarioSesionDto
                {
                    Id = user.Id,
                    IdUser = user.IdUser,
                    IdRol = user.IdRol,
                    RolCode = user.IdRolNavigation.Code,
                    IdDocente = user.Docente?.Id,
                    Cedula = user.Cedula,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Genero = "M",
                    Activo = user.Activo
                }
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
        {
            var exists = await _context.Usuarios.AnyAsync(u => u.Nombre == dto.Email || (dto.Cedula != null && u.Cedula == dto.Cedula))
                         || await _context.AuthUsers.AnyAsync(a => a.Email == dto.Email.ToLower());
            if (exists)
            {
                return Conflict(new
                {
                    status = 409,
                    error = "CONFLICT",
                    message = "Ya existe un usuario con este correo o cédula."
                });
            }

            var rol = await _context.Rols.FindAsync(dto.IdRol);
            if (rol == null)
            {
                return BadRequest(new
                {
                    status = 400,
                    error = "BAD_REQUEST",
                    message = "El rol especificado no existe."
                });
            }

            var newUserId = Guid.NewGuid();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var authUser = new AuthUser
            {
                Id = newUserId,
                Email = dto.Email.Trim().ToLower(),
                EncryptedPassword = hashedPassword,
                EmailConfirmedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AuthUsers.Add(authUser);

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                IdUser = newUserId,
                IdRol = dto.IdRol,
                Cedula = dto.Cedula?.Trim(),
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                FechaNacimiento = dto.FechaNacimiento,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var tokenFake = Guid.NewGuid().ToString("N");
            var refreshFake = Guid.NewGuid().ToString("N");

            return StatusCode(201, new AuthResponseDto
            {
                AccessToken = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{tokenFake}",
                RefreshToken = refreshFake,
                ExpiresIn = 3600,
                Usuario = new UsuarioSesionDto
                {
                    Id = usuario.Id,
                    IdUser = usuario.IdUser,
                    IdRol = usuario.IdRol,
                    RolCode = rol.Code,
                    IdDocente = null,
                    Cedula = usuario.Cedula,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Genero = dto.Genero,
                    Activo = usuario.Activo
                }
            });
        }

        [HttpPost("refresh")]
        public ActionResult Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var tokenFake = Guid.NewGuid().ToString("N");
            var refreshFake = Guid.NewGuid().ToString("N");

            return Ok(new
            {
                access_token = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{tokenFake}",
                refresh_token = refreshFake,
                expires_in = 3600
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return NoContent();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetPerfilActual()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            Usuario? user = null;

            if (!string.IsNullOrEmpty(authHeader))
            {
                var cleanToken = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7).Trim() : authHeader.Trim();
                if (cleanToken.StartsWith("UNHSJM."))
                {
                    var parts = cleanToken.Substring("UNHSJM.".Length).Split('.');
                    if (parts.Length > 0 && Guid.TryParse(parts[0], out var tokenUserId))
                    {
                        user = await _context.Usuarios
                            .Include(u => u.IdRolNavigation)
                            .Include(u => u.Docente)
                            .Include(u => u.AuthUser)
                            .FirstOrDefaultAsync(u => u.Activo && u.Id == tokenUserId);
                    }
                }
            }

            if (user == null && Guid.TryParse(Request.Query["idUsuario"], out var qUserId))
            {
                user = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .Include(u => u.Docente)
                    .Include(u => u.AuthUser)
                    .FirstOrDefaultAsync(u => u.Activo && u.Id == qUserId);
            }

            if (user == null)
            {
                user = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .Include(u => u.Docente)
                    .Include(u => u.AuthUser)
                    .FirstOrDefaultAsync(u => u.Activo);
            }

            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = "Perfil de usuario no encontrado."
                });
            }

            return Ok(new UserProfileDto
            {
                IdUsuario = user.Id,
                IdUser = user.IdUser,
                Email = user.AuthUser?.Email ?? $"{user.Nombre.ToLower()}@unhsjm.edu.ni",
                Cedula = user.Cedula,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Genero = "M",
                FechaNacimiento = user.FechaNacimiento,
                Rol = new RolResumenDto
                {
                    Id = user.IdRolNavigation.Id,
                    Code = user.IdRolNavigation.Code,
                    Nombre = user.IdRolNavigation.Nombre
                },
                Docente = user.Docente != null ? new DocenteResumenPerfilDto
                {
                    Id = user.Docente.Id,
                    CodigoEmpleado = user.Docente.CodigoEmpleado,
                    IdDireccion = user.Docente.IdDireccion,
                    TipoContratacion = "FIJO"
                } : null,
                Permisos = new List<PermisoPerfilDto>()
            });
        }
    }
}
