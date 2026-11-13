using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class CalendarioSemanal
{
    public Guid Id { get; set; }

    public Guid IdMateriaPorGrupo { get; set; }

    public Guid IdAula { get; set; }

    public short IdDia { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool Activo { get; set; }

    public virtual Aula IdAulaNavigation { get; set; } = null!;

    public virtual Dium IdDiaNavigation { get; set; } = null!;

    public virtual MateriaPorGrupo IdMateriaPorGrupoNavigation { get; set; } = null!;
}
