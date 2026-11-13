using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Estudiantes
{
    public class EstudianteFilterDto
    {
        public string? Search { get; set; }
        public string? CodigoUnico { get; set; }
        public string? Carnet { get; set; }
        public Guid? IdCarrera { get; set; }
        public Guid? IdPlanAcademico { get; set; }
        public bool? Activo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class EstudianteItemDto
    {
        public Guid Id { get; set; }
        public string CodigoUnico { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string? Cedula { get; set; }
        public string Nombres { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public DateOnly FechaIngreso { get; set; }
        public Guid IdCarrera { get; set; }
        public string CarreraNombre { get; set; } = null!;
        public Guid IdPlanAcademico { get; set; }
        public string PlanCodigo { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class EstudianteDetalleDto
    {
        public Guid Id { get; set; }
        public string CodigoUnico { get; set; } = null!;
        public Guid IdCarrera { get; set; }
        public Guid IdPlanAcademico { get; set; }
        public string Carnet { get; set; } = null!;
        public string? Cedula { get; set; }
        public string Nombres { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public DateOnly FechaIngreso { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateEstudianteDto
    {
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string CodigoUnico { get; set; } = null!;

        [Required]
        public Guid IdCarrera { get; set; }

        [Required]
        public Guid IdPlanAcademico { get; set; }

        [Required]
        [StringLength(20)]
        public string Carnet { get; set; } = null!;

        [StringLength(20)]
        public string? Cedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = null!;

        [Required]
        [StringLength(60)]
        public string PrimerApellido { get; set; } = null!;

        [StringLength(60)]
        public string? SegundoApellido { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Correo { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public DateOnly FechaIngreso { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateEstudianteDto
    {
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string CodigoUnico { get; set; } = null!;

        [Required]
        public Guid IdCarrera { get; set; }

        [Required]
        public Guid IdPlanAcademico { get; set; }

        [Required]
        [StringLength(20)]
        public string Carnet { get; set; } = null!;

        [StringLength(20)]
        public string? Cedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = null!;

        [Required]
        [StringLength(60)]
        public string PrimerApellido { get; set; } = null!;

        [StringLength(60)]
        public string? SegundoApellido { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Correo { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [Required]
        public DateOnly FechaIngreso { get; set; }

        public bool Activo { get; set; }
    }
}
