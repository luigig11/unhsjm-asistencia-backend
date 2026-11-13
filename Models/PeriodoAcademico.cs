using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class PeriodoAcademico
{
    public Guid Id { get; set; }

    public Guid IdTipoPeriodoAcademico { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short Orden { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetallePlanAcademico> DetallePlanAcademicos { get; set; } = new List<DetallePlanAcademico>();

    public virtual TipoPeriodoAcademico IdTipoPeriodoAcademicoNavigation { get; set; } = null!;
}
