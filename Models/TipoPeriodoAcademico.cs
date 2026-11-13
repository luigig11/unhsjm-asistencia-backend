using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class TipoPeriodoAcademico
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short DuracionMesesMinimo { get; set; }

    public short DuracionMesesMaximo { get; set; }

    public short DuracionHorasMinimo { get; set; }

    public short DuracionHorasMaximo { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<PeriodoAcademico> PeriodoAcademicos { get; set; } = new List<PeriodoAcademico>();

    public virtual ICollection<PlanAcademico> PlanAcademicos { get; set; } = new List<PlanAcademico>();
}
