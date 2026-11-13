using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Feriado
{
    public Guid Id { get; set; }

    public Guid IdPeriodoEscolar { get; set; }

    public Guid IdTipoEvento { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public bool Activo { get; set; }

    public virtual PeriodoEscolar IdPeriodoEscolarNavigation { get; set; } = null!;

    public virtual TipoEvento IdTipoEventoNavigation { get; set; } = null!;
}
