using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class MateriaPorGrupo
{
    public Guid Id { get; set; }

    public Guid IdGrupo { get; set; }

    public Guid IdMallaCurricular { get; set; }

    public Guid IdDocente { get; set; }

    public short CupoLimite { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CalendarioSemanal> CalendarioSemanals { get; set; } = new List<CalendarioSemanal>();

    public virtual ICollection<Calendario> Calendarios { get; set; } = new List<Calendario>();

    public virtual Docente IdDocenteNavigation { get; set; } = null!;

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual MallaCurricular IdMallaCurricularNavigation { get; set; } = null!;

    public virtual ICollection<InscripcionGrupo> InscripcionGrupos { get; set; } = new List<InscripcionGrupo>();
}
