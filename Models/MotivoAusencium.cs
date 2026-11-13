using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class MotivoAusencium
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool JustificaInasistencia { get; set; }

    public string AplicaA { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Asistencium> Asistencia { get; set; } = new List<Asistencium>();

    public virtual ICollection<AsistenciaDetalle> AsistenciaDetalles { get; set; } = new List<AsistenciaDetalle>();
}
