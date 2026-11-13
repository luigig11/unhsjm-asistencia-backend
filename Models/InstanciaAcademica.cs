using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class InstanciaAcademica
{
    public Guid Id { get; set; }

    public Guid IdPeriodoEscolar { get; set; }

    public Guid IdDetallePlanAcademico { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

    public virtual DetallePlanAcademico IdDetallePlanAcademicoNavigation { get; set; } = null!;

    public virtual PeriodoEscolar IdPeriodoEscolarNavigation { get; set; } = null!;
}
