using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class InscripcionGrupo
{
    public Guid Id { get; set; }

    public Guid IdMateriaPorGrupo { get; set; }

    public Guid IdEstudiante { get; set; }

    public DateOnly FechaInscripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AsistenciaDetalle> AsistenciaDetalles { get; set; } = new List<AsistenciaDetalle>();

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual MateriaPorGrupo IdMateriaPorGrupoNavigation { get; set; } = null!;
}
