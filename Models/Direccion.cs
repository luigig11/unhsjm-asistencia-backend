using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Direccion
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Carrera> Carreras { get; set; } = new List<Carrera>();

    public virtual ICollection<Docente> Docentes { get; set; } = new List<Docente>();
}
