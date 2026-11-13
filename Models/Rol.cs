using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Rol
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
