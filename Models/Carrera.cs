using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Carrera
{
    public Guid Id { get; set; }

    public Guid IdDireccion { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();

    public virtual Direccion IdDireccionNavigation { get; set; } = null!;

    public virtual ICollection<PlanAcademico> PlanAcademicos { get; set; } = new List<PlanAcademico>();
}
