using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Models;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Auth;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; //BORRAR
        private readonly ICommandInvoker _commandInvoker;
        private readonly RegisterUserCommand _registerUserCommand;

        public AuthController(AppDbContext context, ICommandInvoker commandInvoker, RegisterUserCommand registerUserCommand)
        {
            _context = context; //BORRAR
            _commandInvoker = commandInvoker;
            _registerUserCommand = registerUserCommand;
        }

        //[HttpPost("login")]
        //public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
        //{
        //    var emailInput = dto.Email.Trim().ToLower();

            
        //    var authUser = await _context.AuthUsers
        //        .FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == emailInput);

        //    Usuario? user = null;

        //    if (authUser != null)
        //    {
        //        user = await _context.Usuarios
        //            .Include(u => u.IdRolNavigation)
        //            .Include(u => u.Docente)
        //            .Include(u => u.AuthUser)
        //            .FirstOrDefaultAsync(u => u.Activo && u.IdUser == authUser.Id);
        //    }

            
        //    if (user == null)
        //    {
        //        var alias = emailInput.Contains('@') ? emailInput.Split('@')[0] : emailInput;
        //        user = await _context.Usuarios
        //            .Include(u => u.IdRolNavigation)
        //            .Include(u => u.Docente)
        //            .Include(u => u.AuthUser)
        //            .FirstOrDefaultAsync(u => u.Activo && (
        //                u.Nombre.ToLower() == alias || 
        //                u.Nombre.ToLower() == emailInput || 
        //                (u.Cedula != null && u.Cedula == dto.Email.Trim())
        //            ));

        //        if (user?.IdUser != null && authUser == null)
        //        {
        //            authUser = await _context.AuthUsers.FindAsync(user.IdUser);
        //        }
        //    }

        //    if (user == null)
        //    {
        //        return Unauthorized(new
        //        {
        //            status = 401,
        //            error = "UNAUTHORIZED",
        //            message = "Credenciales incorrectas o usuario inactivo."
        //        });
        //    }

            
        //    if (authUser?.EncryptedPassword != null && !string.IsNullOrWhiteSpace(dto.Password))
        //    {
        //        bool passwordMatches = false;
        //        try
        //        {
        //            passwordMatches = BCrypt.Net.BCrypt.Verify(dto.Password, authUser.EncryptedPassword);
        //        }
        //        catch
        //        {
        //            passwordMatches = (dto.Password == authUser.EncryptedPassword);
        //        }

        //        if (!passwordMatches)
        //        {
        //            return Unauthorized(new
        //            {
        //                status = 401,
        //                error = "UNAUTHORIZED",
        //                message = "Contraseña incorrecta."
        //            });
        //        }
        //    }

        //    var tokenFake = Guid.NewGuid().ToString("N");
        //    var refreshFake = Guid.NewGuid().ToString("N");

        //    return Ok(new AuthResponseDto
        //    {
        //        AccessToken = $"UNHSJM.{user.Id}.{tokenFake}",
        //        RefreshToken = refreshFake,
        //        ExpiresIn = 3600,
        //        Usuario = new UsuarioSesionDto
        //        {
        //            Id = user.Id,
        //            IdUser = user.IdUser,
        //            IdRol = user.IdRol,
        //            RolCode = user.IdRolNavigation.Code,
        //            IdDocente = user.Docente?.Id,
        //            Cedula = user.Cedula,
        //            Nombre = user.Nombre,
        //            Apellido = user.Apellido,
        //            Genero = "M",
        //            Activo = user.Activo
        //        }
        //    });
        //}

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _commandInvoker.InvokeAsync(_registerUserCommand.WithRequest(dto), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        //[HttpPost("refresh")]
        //public ActionResult Refresh([FromBody] RefreshTokenRequestDto dto)
        //{
        //    var tokenFake = Guid.NewGuid().ToString("N");
        //    var refreshFake = Guid.NewGuid().ToString("N");

        //    return Ok(new
        //    {
        //        access_token = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{tokenFake}",
        //        refresh_token = refreshFake,
        //        expires_in = 3600
        //    });
        //}

        //[HttpPost("logout")]
        //public IActionResult Logout()
        //{
        //    return NoContent();
        //}

        //[HttpGet("me")]
        //public async Task<ActionResult<UserProfileDto>> GetPerfilActual()
        //{
        //    var authHeader = Request.Headers["Authorization"].ToString();
        //    Usuario? user = null;

        //    if (!string.IsNullOrEmpty(authHeader))
        //    {
        //        var cleanToken = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7).Trim() : authHeader.Trim();
        //        if (cleanToken.StartsWith("UNHSJM."))
        //        {
        //            var parts = cleanToken.Substring("UNHSJM.".Length).Split('.');
        //            if (parts.Length > 0 && Guid.TryParse(parts[0], out var tokenUserId))
        //            {
        //                user = await _context.Usuarios
        //                    .Include(u => u.IdRolNavigation)
        //                    .Include(u => u.Docente)
        //                    .Include(u => u.AuthUser)
        //                    .FirstOrDefaultAsync(u => u.Activo && u.Id == tokenUserId);
        //            }
        //        }
        //    }

        //    if (user == null && Guid.TryParse(Request.Query["idUsuario"], out var qUserId))
        //    {
        //        user = await _context.Usuarios
        //            .Include(u => u.IdRolNavigation)
        //            .Include(u => u.Docente)
        //            .Include(u => u.AuthUser)
        //            .FirstOrDefaultAsync(u => u.Activo && u.Id == qUserId);
        //    }

        //    if (user == null)
        //    {
        //        user = await _context.Usuarios
        //            .Include(u => u.IdRolNavigation)
        //            .Include(u => u.Docente)
        //            .Include(u => u.AuthUser)
        //            .FirstOrDefaultAsync(u => u.Activo);
        //    }

        //    if (user == null)
        //    {
        //        return NotFound(new
        //        {
        //            status = 404,
        //            error = "NOT_FOUND",
        //            message = "Perfil de usuario no encontrado."
        //        });
        //    }

        //    return Ok(new UserProfileDto
        //    {
        //        IdUsuario = user.Id,
        //        IdUser = user.IdUser,
        //        Email = user.AuthUser?.Email ?? $"{user.Nombre.ToLower()}@unhsjm.edu.ni",
        //        Cedula = user.Cedula,
        //        Nombre = user.Nombre,
        //        Apellido = user.Apellido,
        //        Genero = "M",
        //        FechaNacimiento = user.FechaNacimiento,
        //        Rol = new RolResumenDto
        //        {
        //            Id = user.IdRolNavigation.Id,
        //            Code = user.IdRolNavigation.Code,
        //            Nombre = user.IdRolNavigation.Nombre
        //        },
        //        Docente = user.Docente != null ? new DocenteResumenPerfilDto
        //        {
        //            Id = user.Docente.Id,
        //            CodigoEmpleado = user.Docente.CodigoEmpleado,
        //            IdDireccion = user.Docente.IdDireccion,
        //            TipoContratacion = "FIJO"
        //        } : null,
        //        Permisos = new List<PermisoPerfilDto>()
        //    });
        //}
    }
}
