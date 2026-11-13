using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Estudiante
{
    public Guid Id { get; set; }

    public Guid IdCarrera { get; set; }

    public Guid IdPlanAcademico { get; set; }

    public string Carnet { get; set; } = null!;

    public string? Cedula { get; set; }

    public string Nombres { get; set; } = null!;

    public string PrimerApellido { get; set; } = null!;

    public string? SegundoApellido { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public bool Activo { get; set; }

    public string CodigoUnico { get; set; } = null!;

    public virtual Carrera IdCarreraNavigation { get; set; } = null!;

    public virtual PlanAcademico IdPlanAcademicoNavigation { get; set; } = null!;

    public virtual ICollection<InscripcionGrupo> InscripcionGrupos { get; set; } = new List<InscripcionGrupo>();
}
