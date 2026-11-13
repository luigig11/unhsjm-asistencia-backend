using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Aula
{
    public Guid Id { get; set; }

    public Guid IdTipoAula { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short Capacidad { get; set; }

    public string? Ubicacion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CalendarioSemanal> CalendarioSemanals { get; set; } = new List<CalendarioSemanal>();

    public virtual ICollection<Calendario> Calendarios { get; set; } = new List<Calendario>();

    public virtual TipoAula IdTipoAulaNavigation { get; set; } = null!;
}
