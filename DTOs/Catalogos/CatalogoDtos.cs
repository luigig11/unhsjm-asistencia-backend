using System.ComponentModel.DataAnnotations;

namespace unhsjm_asistencia_backend.DTOs.Catalogos
{
    public class DireccionDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateDireccionDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateDireccionDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }

    public class CarreraDto
    {
        public Guid Id { get; set; }
        public Guid IdDireccion { get; set; }
        public string DireccionNombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateCarreraDto
    {
        [Required]
        public Guid IdDireccion { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateCarreraDto
    {
        [Required]
        public Guid IdDireccion { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }

    public class ModalidadDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateModalidadDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateModalidadDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }

    public class TipoPeriodoDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public short DuracionMesesMinimo { get; set; }
        public short DuracionMesesMaximo { get; set; }
        public short DuracionHorasMinimo { get; set; }
        public short DuracionHorasMaximo { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateTipoPeriodoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public short DuracionMesesMinimo { get; set; }
        public short DuracionMesesMaximo { get; set; }
        public short DuracionHorasMinimo { get; set; }
        public short DuracionHorasMaximo { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class UpdateTipoPeriodoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public short DuracionMesesMinimo { get; set; }
        public short DuracionMesesMaximo { get; set; }
        public short DuracionHorasMinimo { get; set; }
        public short DuracionHorasMaximo { get; set; }
        public bool Activo { get; set; }
    }

    public class PeriodoAcademicoDto
    {
        public Guid Id { get; set; }
        public Guid IdTipoPeriodoAcademico { get; set; }
        public string TipoPeriodoNombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public short Orden { get; set; }
        public bool Activo { get; set; }
    }

    public class CreatePeriodoAcademicoDto
    {
        [Required]
        public Guid IdTipoPeriodoAcademico { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Orden { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdatePeriodoAcademicoDto
    {
        [Required]
        public Guid IdTipoPeriodoAcademico { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Orden { get; set; }

        public bool Activo { get; set; }
    }

    public class CicloAcademicoDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public short Orden { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateCicloAcademicoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, 10)]
        public short Orden { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateCicloAcademicoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, 10)]
        public short Orden { get; set; }

        public bool Activo { get; set; }
    }

    public class MateriaDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateMateriaDto
    {
        [Required]
        [StringLength(25)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateMateriaDto
    {
        [Required]
        [StringLength(25)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }

    public class CentroUniversitarioDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateCentroDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Ubicacion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateCentroDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Ubicacion { get; set; }

        public bool Activo { get; set; }
    }

    public class TipoAulaDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateTipoAulaDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateTipoAulaDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }

    public class AulaDto
    {
        public Guid Id { get; set; }
        public Guid IdTipoAula { get; set; }
        public string TipoAulaNombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public short Capacidad { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
    }

    public class CreateAulaDto
    {
        [Required]
        public Guid IdTipoAula { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Capacidad { get; set; }

        [StringLength(150)]
        public string? Ubicacion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class UpdateAulaDto
    {
        [Required]
        public Guid IdTipoAula { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Range(1, short.MaxValue)]
        public short Capacidad { get; set; }

        [StringLength(150)]
        public string? Ubicacion { get; set; }

        public bool Activo { get; set; }
    }

    public class DiaDto
    {
        public short Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class TipoEventoDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateTipoEventoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateTipoEventoDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }

    public class MotivoAusenciaDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool JustificaInasistencia { get; set; }
        public string AplicaA { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class CreateMotivoAusenciaDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool JustificaInasistencia { get; set; }

        [Required]
        public string AplicaA { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }

    public class UpdateMotivoAusenciaDto
    {
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool JustificaInasistencia { get; set; }

        [Required]
        public string AplicaA { get; set; } = null!;

        public bool Activo { get; set; }
    }
}
