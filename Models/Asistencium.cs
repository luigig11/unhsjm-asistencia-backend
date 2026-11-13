using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Asistencium
{
    public Guid Id { get; set; }

    public Guid IdCalendario { get; set; }

    public Guid? IdDocenteSustituto { get; set; }

    public bool? DocenteAsignadoAsistio { get; set; }

    public Guid? IdMotivoAusencia { get; set; }

    public DateTime? FechaRegistroDocente { get; set; }

    public virtual ICollection<AsistenciaDetalle> AsistenciaDetalles { get; set; } = new List<AsistenciaDetalle>();

    public virtual Calendario IdCalendarioNavigation { get; set; } = null!;

    public virtual Docente? IdDocenteSustitutoNavigation { get; set; }

    public virtual MotivoAusencium? IdMotivoAusenciaNavigation { get; set; }
}
