using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class CicloAcademico
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short Orden { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetallePlanAcademico> DetallePlanAcademicos { get; set; } = new List<DetallePlanAcademico>();
}
