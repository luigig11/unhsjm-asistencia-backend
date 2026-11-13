using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class AsistenciaDetalle
{
    public Guid Id { get; set; }

    public Guid IdAsistencia { get; set; }

    public Guid IdInscripcionGrupo { get; set; }

    public bool Asistio { get; set; }

    public Guid? IdMotivoAusencia { get; set; }

    public DateTime? FechaRegistroEstudiante { get; set; }

    public virtual Asistencium IdAsistenciaNavigation { get; set; } = null!;

    public virtual InscripcionGrupo IdInscripcionGrupoNavigation { get; set; } = null!;

    public virtual MotivoAusencium? IdMotivoAusenciaNavigation { get; set; }
}
