using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Calendario
{
    public Guid Id { get; set; }

    public Guid IdMateriaPorGrupo { get; set; }

    public Guid IdAula { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public bool Activo { get; set; }

    public virtual Asistencium? Asistencium { get; set; }

    public virtual Aula IdAulaNavigation { get; set; } = null!;

    public virtual MateriaPorGrupo IdMateriaPorGrupoNavigation { get; set; } = null!;
}
