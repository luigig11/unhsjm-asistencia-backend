using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Planes
{
    public class PlanFilterDto
    {
        public Guid? IdDireccion { get; set; }
        public Guid? IdCarrera { get; set; }
        public Guid? IdModalidad { get; set; }
        public Guid? IdCentroUniversitario { get; set; }
        public bool? Activo { get; set; }
    }

    public class PlanItemDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public Guid IdCentroUniversitario { get; set; }
        public string CentroNombre { get; set; } = null!;
        public Guid IdCarrera { get; set; }
        public string CarreraNombre { get; set; } = null!;
        public Guid IdTipoPeriodoAcademico { get; set; }
        public string TipoPeriodoNombre { get; set; } = null!;
        public Guid IdModalidad { get; set; }
        public string ModalidadNombre { get; set; } = null!;
        public short CantidadCicloAcademico { get; set; }
        public short CantidadPeriodoAcademico { get; set; }
        public DateOnly ActivoDesde { get; set; }
        public DateOnly? ActivoHasta { get; set; }
        public bool Activo { get; set; }
    }

    public class PlanDetalleDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public Guid IdCentroUniversitario { get; set; }
        public Guid IdCarrera { get; set; }
        public Guid IdTipoPeriodoAcademico { get; set; }
        public Guid IdModalidad { get; set; }
        public short CantidadCicloAcademico { get; set; }
        public short CantidadPeriodoAcademico { get; set; }
        public DateOnly ActivoDesde { get; set; }
        public DateOnly? ActivoHasta { get; set; }
        public bool Activo { get; set; }
    }

    public class CreatePlanAcademicoDto
    {
        [Required]
        public Guid IdCentroUniversitario { get; set; }

        [Required]
        public Guid IdCarrera { get; set; }

        [Required]
        public Guid IdTipoPeriodoAcademico { get; set; }

        [Required]
        public Guid IdModalidad { get; set; }

        [Required]
        [StringLength(30)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short CantidadCicloAcademico { get; set; }

        [Range(1, short.MaxValue)]
        public short CantidadPeriodoAcademico { get; set; }

        [Required]
        public DateOnly ActivoDesde { get; set; }

        public DateOnly? ActivoHasta { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdatePlanAcademicoDto
    {
        [Required]
        public Guid IdCentroUniversitario { get; set; }

        [Required]
        public Guid IdCarrera { get; set; }

        [Required]
        public Guid IdTipoPeriodoAcademico { get; set; }

        [Required]
        public Guid IdModalidad { get; set; }

        [Required]
        [StringLength(30)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short CantidadCicloAcademico { get; set; }

        [Range(1, short.MaxValue)]
        public short CantidadPeriodoAcademico { get; set; }

        [Required]
        public DateOnly ActivoDesde { get; set; }

        public DateOnly? ActivoHasta { get; set; }

        public bool Activo { get; set; }
    }

    public class PrerequisitoMallaDto
    {
        public Guid Id { get; set; }
        public Guid IdMallaCurricularMateriaPrerequisito { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
    }

    public class MateriaMallaDto
    {
        public Guid IdMallaCurricular { get; set; }
        public Guid IdMateria { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public short Creditos { get; set; }
        public decimal CantidadHorasMateria { get; set; }
        public short EncuentrosTotales { get; set; }
        public short EncuentrosPorSemana { get; set; }
        public bool Obligatoria { get; set; }
        public bool Activo { get; set; }
        public List<PrerequisitoMallaDto> Prerequisitos { get; set; } = new();
    }

    public class BloqueMallaDto
    {
        public Guid IdDetallePlanAcademico { get; set; }
        public Guid IdPeriodoAcademico { get; set; }
        public string PeriodoNombre { get; set; } = null!;
        public short OrdenPeriodo { get; set; }
        public short DuracionSemanas { get; set; }
        public short DuracionMeses { get; set; }
        public Guid? IdDetallePredecesor { get; set; }
        public List<MateriaMallaDto> Materias { get; set; } = new();
    }

    public class CicloMallaDto
    {
        public Guid IdCicloAcademico { get; set; }
        public string CicloNombre { get; set; } = null!;
        public short OrdenCiclo { get; set; }
        public List<BloqueMallaDto> Bloques { get; set; } = new();
    }

    public class PlanResumenMallaDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string CarreraNombre { get; set; } = null!;
        public string ModalidadNombre { get; set; } = null!;
    }

    public class MallaCompletaDto
    {
        public PlanResumenMallaDto Plan { get; set; } = null!;
        public List<CicloMallaDto> Ciclos { get; set; } = new();
    }

    public class DetallePlanDto
    {
        public Guid Id { get; set; }
        public Guid IdPlanAcademico { get; set; }
        public Guid IdCicloAcademico { get; set; }
        public string CicloNombre { get; set; } = null!;
        public Guid IdPeriodoAcademico { get; set; }
        public string PeriodoNombre { get; set; } = null!;
        public short DuracionSemanas { get; set; }
        public short DuracionMeses { get; set; }
        public Guid? IdDetallePredecesor { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateDetallePlanDto
    {
        [Required]
        public Guid IdPlanAcademico { get; set; }

        [Required]
        public Guid IdCicloAcademico { get; set; }

        [Required]
        public Guid IdPeriodoAcademico { get; set; }

        [Range(1, 52)]
        public short DuracionSemanas { get; set; }

        [Range(1, 12)]
        public short DuracionMeses { get; set; }

        public Guid? IdDetallePredecesor { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateDetallePlanDto
    {
        [Required]
        public Guid IdCicloAcademico { get; set; }

        [Required]
        public Guid IdPeriodoAcademico { get; set; }

        [Range(1, 52)]
        public short DuracionSemanas { get; set; }

        [Range(1, 12)]
        public short DuracionMeses { get; set; }

        public Guid? IdDetallePredecesor { get; set; }

        public bool Activo { get; set; }
    }

    public class CreateMallaCurricularDto
    {
        [Required]
        public Guid IdDetallePlanAcademico { get; set; }

        [Required]
        public Guid IdMateria { get; set; }

        [Range(1, short.MaxValue)]
        public short EncuentrosTotales { get; set; }

        [Range(1, 7)]
        public short EncuentrosPorSemana { get; set; }

        public decimal CantidadHorasMateria { get; set; }

        [Range(1, short.MaxValue)]
        public short Creditos { get; set; }

        public bool Obligatoria { get; set; } = true;

        public bool Activo { get; set; } = true;

        public List<Guid>? IdsMallaPrerequisitos { get; set; }
    }

    public class UpdateMallaCurricularDto
    {
        [Required]
        public Guid IdDetallePlanAcademico { get; set; }

        [Required]
        public Guid IdMateria { get; set; }

        [Range(1, short.MaxValue)]
        public short EncuentrosTotales { get; set; }

        [Range(1, 7)]
        public short EncuentrosPorSemana { get; set; }

        public decimal CantidadHorasMateria { get; set; }

        [Range(1, short.MaxValue)]
        public short Creditos { get; set; }

        public bool Obligatoria { get; set; }

        public bool Activo { get; set; }

        public List<Guid>? IdsMallaPrerequisitos { get; set; }
    }

    public class CreatePrerequisitoDto
    {
        [Required]
        public Guid IdMallaCurricularMateria { get; set; }

        [Required]
        public Guid IdMallaCurricularMateriaPrerequisito { get; set; }

        public bool Activo { get; set; } = true;
    }
}
