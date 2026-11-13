using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Calendario
{
    public class PeriodoEscolarDto
    {
        public Guid Id { get; set; }
        public string NombrePeriodoEscolar { get; set; } = null!;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool Activo { get; set; }
    }

    public class CreatePeriodoEscolarDto
    {
        [Required]
        [StringLength(50)]
        public string NombrePeriodoEscolar { get; set; } = null!;

        [Required]
        public DateOnly FechaInicio { get; set; }

        [Required]
        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdatePeriodoEscolarDto
    {
        [Required]
        [StringLength(50)]
        public string NombrePeriodoEscolar { get; set; } = null!;

        [Required]
        public DateOnly FechaInicio { get; set; }

        [Required]
        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; }
    }

    public class FeriadoDto
    {
        public Guid Id { get; set; }
        public Guid IdPeriodoEscolar { get; set; }
        public Guid IdTipoEvento { get; set; }
        public string TipoEventoCodigo { get; set; } = null!;
        public string TipoEventoNombre { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateFeriadoDto
    {
        [Required]
        public Guid IdPeriodoEscolar { get; set; }

        [Required]
        public Guid IdTipoEvento { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateFeriadoDto
    {
        [Required]
        public Guid IdPeriodoEscolar { get; set; }

        [Required]
        public Guid IdTipoEvento { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; }
    }

    public class InstanciaAcademicaDto
    {
        public Guid Id { get; set; }
        public Guid IdPeriodoEscolar { get; set; }
        public string NombrePeriodoEscolar { get; set; } = null!;
        public Guid IdDetallePlanAcademico { get; set; }
        public string PlanCodigo { get; set; } = null!;
        public string CicloNombre { get; set; } = null!;
        public string PeriodoNombre { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateInstanciaDto
    {
        [Required]
        public Guid IdPeriodoEscolar { get; set; }

        [Required]
        public Guid IdDetallePlanAcademico { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        public DateOnly FechaInicio { get; set; }

        [Required]
        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateInstanciaDto
    {
        [Required]
        public Guid IdPeriodoEscolar { get; set; }

        [Required]
        public Guid IdDetallePlanAcademico { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        public DateOnly FechaInicio { get; set; }

        [Required]
        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; }
    }

    public class GrupoDto
    {
        public Guid Id { get; set; }
        public Guid IdInstanciaAcademica { get; set; }
        public string InstanciaNombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public short Cupo { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateGrupoDto
    {
        [Required]
        public Guid IdInstanciaAcademica { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Cupo { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateGrupoDto
    {
        [Required]
        public Guid IdInstanciaAcademica { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Cupo { get; set; }

        public bool Activo { get; set; }
    }

    public class MateriaPorGrupoDto
    {
        public Guid Id { get; set; }
        public Guid IdGrupo { get; set; }
        public string GrupoCodigo { get; set; } = null!;
        public Guid IdMallaCurricular { get; set; }
        public Guid IdMateria { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public Guid IdDocente { get; set; }
        public string DocenteNombreCompleto { get; set; } = null!;
        public short CupoLimite { get; set; }
        public int TotalInscritos { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateMateriaPorGrupoDto
    {
        [Required]
        public Guid IdGrupo { get; set; }

        [Required]
        public Guid IdMallaCurricular { get; set; }

        [Required]
        public Guid IdDocente { get; set; }

        [Range(1, short.MaxValue)]
        public short CupoLimite { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateMateriaPorGrupoDto
    {
        [Required]
        public Guid IdDocente { get; set; }

        [Range(1, short.MaxValue)]
        public short CupoLimite { get; set; }

        public bool Activo { get; set; }
    }

    public class CalendarioSemanalDto
    {
        public Guid Id { get; set; }
        public Guid IdMateriaPorGrupo { get; set; }
        public Guid IdAula { get; set; }
        public string AulaCodigo { get; set; } = null!;
        public string AulaNombre { get; set; } = null!;
        public short IdDia { get; set; }
        public string DiaNombre { get; set; } = null!;
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateCalendarioSemanalDto
    {
        [Required]
        public Guid IdMateriaPorGrupo { get; set; }

        [Required]
        public Guid IdAula { get; set; }

        [Range(1, 7)]
        public short IdDia { get; set; }

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFin { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateCalendarioSemanalDto
    {
        public Guid? IdMateriaPorGrupo { get; set; }

        [Required]
        public Guid IdAula { get; set; }

        [Range(1, 7)]
        public short IdDia { get; set; }

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFin { get; set; }

        public bool Activo { get; set; }
    }

    public class FechaOmitidaFeriadoDto
    {
        public DateOnly Fecha { get; set; }
        public string MotivoFeriado { get; set; } = null!;
    }

    public class SesionCalendarioDto
    {
        public Guid Id { get; set; }
        public Guid IdMateriaPorGrupo { get; set; }
        public Guid IdAula { get; set; }
        public string AulaCodigo { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public string? EstadoAsistencia { get; set; }
    }

    public class GenerarCalendarioResultDto
    {
        public int SesionesCreadas { get; set; }
        public List<FechaOmitidaFeriadoDto> FechasOmitidasPorFeriado { get; set; } = new();
        public List<SesionCalendarioDto> Sesiones { get; set; } = new();
    }

    public class CreateCalendarioDto
    {
        [Required]
        public Guid IdMateriaPorGrupo { get; set; }

        [Required]
        public Guid IdAula { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateCalendarioDto
    {
        [Required]
        public Guid IdAula { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; }
    }

    public class InscripcionGrupoDto
    {
        public Guid Id { get; set; }
        public Guid IdMateriaPorGrupo { get; set; }
        public Guid IdEstudiante { get; set; }
        public string CodigoUnico { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public DateOnly FechaInscripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateInscripcionGrupoDto
    {
        [Required]
        public Guid IdMateriaPorGrupo { get; set; }

        [Required]
        public Guid IdEstudiante { get; set; }

        public DateOnly? FechaInscripcion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateInscripcionGrupoDto
    {
        public bool Activo { get; set; }
    }
}
