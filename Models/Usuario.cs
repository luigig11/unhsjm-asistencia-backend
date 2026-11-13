using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Usuario
{
    public Guid Id { get; set; }

    public Guid? IdUser { get; set; }

    public Guid IdRol { get; set; }

    public string? Cedula { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public bool Activo { get; set; }

    public virtual Docente? Docente { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual AuthUser? AuthUser { get; set; }
}
