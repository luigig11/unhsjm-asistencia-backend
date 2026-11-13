using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class PlanAcademico
{
    public Guid Id { get; set; }

    public Guid IdCentroUniversitario { get; set; }

    public Guid IdCarrera { get; set; }

    public Guid IdTipoPeriodoAcademico { get; set; }

    public Guid IdModalidad { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short CantidadCicloAcademico { get; set; }

    public short CantidadPeriodoAcademico { get; set; }

    public DateOnly ActivoDesde { get; set; }

    public DateOnly? ActivoHasta { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetallePlanAcademico> DetallePlanAcademicos { get; set; } = new List<DetallePlanAcademico>();

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();

    public virtual Carrera IdCarreraNavigation { get; set; } = null!;

    public virtual CentroUniversitario IdCentroUniversitarioNavigation { get; set; } = null!;

    public virtual Modalidad IdModalidadNavigation { get; set; } = null!;

    public virtual TipoPeriodoAcademico IdTipoPeriodoAcademicoNavigation { get; set; } = null!;
}
