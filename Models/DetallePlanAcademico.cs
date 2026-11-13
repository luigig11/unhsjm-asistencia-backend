using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class DetallePlanAcademico
{
    public Guid Id { get; set; }

    public Guid IdPlanAcademico { get; set; }

    public Guid IdCicloAcademico { get; set; }

    public Guid IdPeriodoAcademico { get; set; }

    public short DuracionSemanas { get; set; }

    public short DuracionMeses { get; set; }

    public Guid? IdDetallePredecesor { get; set; }

    public bool Activo { get; set; }

    public virtual CicloAcademico IdCicloAcademicoNavigation { get; set; } = null!;

    public virtual DetallePlanAcademico? IdDetallePredecesorNavigation { get; set; }

    public virtual PeriodoAcademico IdPeriodoAcademicoNavigation { get; set; } = null!;

    public virtual PlanAcademico IdPlanAcademicoNavigation { get; set; } = null!;

    public virtual ICollection<InstanciaAcademica> InstanciaAcademicas { get; set; } = new List<InstanciaAcademica>();

    public virtual ICollection<DetallePlanAcademico> InverseIdDetallePredecesorNavigation { get; set; } = new List<DetallePlanAcademico>();

    public virtual ICollection<MallaCurricular> MallaCurriculars { get; set; } = new List<MallaCurricular>();
}
