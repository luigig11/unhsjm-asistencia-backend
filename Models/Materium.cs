using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Materium
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<MallaCurricular> MallaCurriculars { get; set; } = new List<MallaCurricular>();
}
