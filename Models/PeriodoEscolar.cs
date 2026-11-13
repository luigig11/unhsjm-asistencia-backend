using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class PeriodoEscolar
{
    public Guid Id { get; set; }

    public string NombrePeriodoEscolar { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Feriado> Feriados { get; set; } = new List<Feriado>();

    public virtual ICollection<InstanciaAcademica> InstanciaAcademicas { get; set; } = new List<InstanciaAcademica>();
}
