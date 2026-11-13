using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Dium
{
    public short Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<CalendarioSemanal> CalendarioSemanals { get; set; } = new List<CalendarioSemanal>();
}
