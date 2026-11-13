using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Docente
{
    public Guid Id { get; set; }

    public Guid IdUsuario { get; set; }

    public Guid IdDireccion { get; set; }

    public string CodigoEmpleado { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Asistencium> Asistencia { get; set; } = new List<Asistencium>();

    public virtual Direccion IdDireccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<MateriaPorGrupo> MateriaPorGrupos { get; set; } = new List<MateriaPorGrupo>();
}
