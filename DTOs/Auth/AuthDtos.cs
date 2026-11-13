using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public Guid IdRol { get; set; }

        public string? Cedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(60)]
        public string Apellido { get; set; } = null!;

        [Required]
        [RegularExpression("^[FM]$")]
        public string Genero { get; set; } = null!;

        public DateOnly? FechaNacimiento { get; set; }
    }

    public class LoginRequestDto
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }

    public class UsuarioSesionDto
    {
        public Guid Id { get; set; }
        public Guid? IdUser { get; set; }
        public Guid IdRol { get; set; }
        public string RolCode { get; set; } = null!;
        public Guid? IdDocente { get; set; }
        public string? Cedula { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Genero { get; set; }
        public bool Activo { get; set; }
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public UsuarioSesionDto Usuario { get; set; } = null!;
    }

    public class RolResumenDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }

    public class DocenteResumenPerfilDto
    {
        public Guid Id { get; set; }
        public string CodigoEmpleado { get; set; } = null!;
        public Guid IdDireccion { get; set; }
        public string TipoContratacion { get; set; } = null!;
    }

    public class PermisoPerfilDto
    {
        public string Entidad { get; set; } = null!;
        public List<string> OperacionesPermitidas { get; set; } = new();
    }

    public class UserProfileDto
    {
        public Guid IdUsuario { get; set; }
        public Guid? IdUser { get; set; }
        public string Email { get; set; } = null!;
        public string? Cedula { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public DateOnly? FechaNacimiento { get; set; }
        public RolResumenDto Rol { get; set; } = null!;
        public DocenteResumenPerfilDto? Docente { get; set; }
        public List<PermisoPerfilDto> Permisos { get; set; } = new();
    }

    public class UsuarioFilterDto
    {
        public string? Search { get; set; }
        public Guid? IdRol { get; set; }
        public bool? Activo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class UsuarioListItemDto
    {
        public Guid Id { get; set; }
        public Guid? IdUser { get; set; }
        public Guid IdRol { get; set; }
        public string RolNombre { get; set; } = null!;
        public string? Cedula { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Genero { get; set; } = "M";
        public string? FechaNacimiento { get; set; }
        public bool Activo { get; set; }
        public bool TienePerfilDocente { get; set; }
        public Guid? IdDocente { get; set; }
    }

    public class PagedUsuariosDto
    {
        public int Total { get; set; }
        public List<UsuarioListItemDto> Items { get; set; } = new();
    }

    public class UpdateUsuarioDto
    {
        public Guid IdRol { get; set; }
        public string? Cedula { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Genero { get; set; } = "M";
        public DateOnly? FechaNacimiento { get; set; }
        public bool Activo { get; set; }
    }

    public class RolDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
