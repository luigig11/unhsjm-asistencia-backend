using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Docentes
{
    public class DocenteFilterDto
    {
        public string? Search { get; set; }
        public Guid? IdDireccion { get; set; }
        public string? TipoContratacion { get; set; }
        public bool? Activo { get; set; }
    }

    public class DocenteItemDto
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Cedula { get; set; }
        public Guid IdDireccion { get; set; }
        public string DireccionNombre { get; set; } = null!;
        public string CodigoEmpleado { get; set; } = null!;
        public string? Telefono { get; set; }
        public string TipoContratacion { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class DocenteUsuarioResumenDto
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Cedula { get; set; }
        public string Genero { get; set; } = null!;
    }

    public class DocenteDetalleDto
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public DocenteUsuarioResumenDto Usuario { get; set; } = null!;
        public Guid IdDireccion { get; set; }
        public string CodigoEmpleado { get; set; } = null!;
        public string? Telefono { get; set; }
        public string TipoContratacion { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateDocenteDto
    {
        [Required]
        public Guid IdUsuario { get; set; }

        [Required]
        public Guid IdDireccion { get; set; }

        [Required]
        [StringLength(30)]
        public string CodigoEmpleado { get; set; } = null!;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public string TipoContratacion { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateDocenteDto
    {
        [Required]
        public Guid IdDireccion { get; set; }

        [Required]
        [StringLength(30)]
        public string CodigoEmpleado { get; set; } = null!;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public string TipoContratacion { get; set; } = null!;

        public bool Activo { get; set; }
    }
}
