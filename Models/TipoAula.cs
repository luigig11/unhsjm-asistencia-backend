using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class TipoAula
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();
}
