using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class CentroUniversitario
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Ubicacion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<PlanAcademico> PlanAcademicos { get; set; } = new List<PlanAcademico>();
}
