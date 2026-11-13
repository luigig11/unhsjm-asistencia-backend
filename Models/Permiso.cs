using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Permiso
{
    public Guid Id { get; set; }

    public Guid IdRol { get; set; }

    public string Entidad { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public bool Activo { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;
}
