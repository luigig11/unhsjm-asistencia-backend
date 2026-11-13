using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Asistencia
{
    public class PeriodoActivoResumenDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class SesionHoyItemDto
    {
        public Guid IdCalendario { get; set; }
        public string MateriaNombre { get; set; } = null!;
        public string GrupoCodigo { get; set; } = null!;
        public string AulaCodigo { get; set; } = null!;
        public string DocenteNombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string EstadoActa { get; set; } = null!;
    }

    public class EventoProximoItemDto
    {
        public Guid IdFeriado { get; set; }
        public string Nombre { get; set; } = null!;
        public string TipoEvento { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class DashboardResumenDto
    {
        public PeriodoActivoResumenDto? PeriodoEscolarActivo { get; set; }
        public int TotalGruposOfertados { get; set; }
        public int TotalSesionesHoy { get; set; }
        public int AlertasAusenciaDocenteHoy { get; set; }
        public List<SesionHoyItemDto> SesionesHoy { get; set; } = new();
        public List<EventoProximoItemDto> ProximosEventos { get; set; } = new();
    }

    public class DocenteSesionHoyDto
    {
        public Guid IdCalendario { get; set; }
        public Guid IdMateriaPorGrupo { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public string GrupoCodigo { get; set; } = null!;
        public string AulaCodigo { get; set; } = null!;
        public string? AulaUbicacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Guid? IdAsistencia { get; set; }
        public bool ActaRegistrada { get; set; }
        public bool EsSustituto { get; set; }
    }

    public class DocenteCursoDto
    {
        public Guid IdMateriaPorGrupo { get; set; }
        public string InstanciaNombre { get; set; } = null!;
        public string GrupoCodigo { get; set; } = null!;
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public int TotalInscritos { get; set; }
        public short CupoLimite { get; set; }
    }

    public class SesionDetalleActaDto
    {
        public Guid IdCalendario { get; set; }
        public Guid IdMateriaPorGrupo { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public string GrupoCodigo { get; set; } = null!;
        public string AulaCodigo { get; set; } = null!;
        public string? AulaNombre { get; set; }
        public string DocenteTitularNombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class CabeceraAsistenciaActaDto
    {
        public Guid? Id { get; set; }
        public bool? DocenteAsignadoAsistio { get; set; }
        public Guid? IdDocenteSustituto { get; set; }
        public string? DocenteSustitutoNombre { get; set; }
        public Guid? IdMotivoAusencia { get; set; }
        public DateTime? FechaRegistroDocente { get; set; }
    }

    public class EstudianteActaItemDto
    {
        public Guid? IdAsistenciaDetalle { get; set; }
        public Guid IdInscripcionGrupo { get; set; }
        public Guid IdEstudiante { get; set; }
        public string CodigoUnico { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public bool Asistio { get; set; }
        public Guid? IdMotivoAusencia { get; set; }
        public DateTime? FechaRegistroEstudiante { get; set; }
    }

    public class ActaAsistenciaSesionDto
    {
        public SesionDetalleActaDto Sesion { get; set; } = null!;
        public CabeceraAsistenciaActaDto CabeceraAsistencia { get; set; } = null!;
        public List<EstudianteActaItemDto> Estudiantes { get; set; } = new();
    }

    public class DetalleGuardarAsistenciaDto
    {
        [Required]
        public Guid IdInscripcionGrupo { get; set; }

        [Required]
        public bool Asistio { get; set; }

        public Guid? IdMotivoAusencia { get; set; }
    }

    public class GuardarActaAsistenciaDto
    {
        [Required]
        public bool DocenteAsignadoAsistio { get; set; }

        public Guid? IdDocenteSustituto { get; set; }
        public Guid? IdMotivoAusencia { get; set; }

        [Required]
        public List<DetalleGuardarAsistenciaDto> Detalles { get; set; } = new();
    }

    public class ResumenGuardarActaDto
    {
        public int Presentes { get; set; }
        public int Ausentes { get; set; }
        public int Justificados { get; set; }
    }

    public class ResultadoGuardarActaDto
    {
        public Guid IdAsistencia { get; set; }
        public Guid IdCalendario { get; set; }
        public bool DocenteAsignadoAsistio { get; set; }
        public Guid? IdDocenteSustituto { get; set; }
        public Guid? IdMotivoAusencia { get; set; }
        public DateTime FechaRegistroDocente { get; set; }
        public ResumenGuardarActaDto Resumen { get; set; } = null!;
    }

    public class UpdateAsistenciaDetalleDto
    {
        [Required]
        public bool Asistio { get; set; }
        public Guid? IdMotivoAusencia { get; set; }
    }

    public class CursoSabanaDto
    {
        public Guid IdMateriaPorGrupo { get; set; }
        public string MateriaCodigo { get; set; } = null!;
        public string MateriaNombre { get; set; } = null!;
        public string GrupoCodigo { get; set; } = null!;
        public string DocenteNombre { get; set; } = null!;
        public string InstanciaNombre { get; set; } = null!;
    }

    public class SesionColumnaDto
    {
        public Guid IdCalendario { get; set; }
        public int NumeroEncuentro { get; set; }
        public DateTime FechaInicio { get; set; }
        public bool ActaRegistrada { get; set; }
    }

    public class MarcajeDto
    {
        public Guid IdCalendario { get; set; }
        public string Estado { get; set; } = null!;
        public string? MotivoAusenciaNombre { get; set; }
        public DateTime? FechaRegistroEstudiante { get; set; }
    }

    public class EstadisticasAlumnoDto
    {
        public int TotalPresentes { get; set; }
        public int TotalAusentesInjustificados { get; set; }
        public int TotalJustificados { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
        public string AlertaRiesgo { get; set; } = null!;
    }

    public class AlumnoFilaDto
    {
        public Guid IdInscripcionGrupo { get; set; }
        public Guid IdEstudiante { get; set; }
        public string CodigoUnico { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public List<MarcajeDto> Marcajes { get; set; } = new();
        public EstadisticasAlumnoDto Estadisticas { get; set; } = null!;
    }

    public class SabanaAsistenciaDto
    {
        public CursoSabanaDto Curso { get; set; } = null!;
        public List<SesionColumnaDto> SesionesColumnas { get; set; } = new();
        public List<AlumnoFilaDto> AlumnosFilas { get; set; } = new();
    }
}
