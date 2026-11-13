using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class TipoEvento
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Feriado> Feriados { get; set; } = new List<Feriado>();
}
