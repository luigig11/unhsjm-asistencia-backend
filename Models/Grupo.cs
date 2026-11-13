using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class Grupo
{
    public Guid Id { get; set; }

    public Guid IdInstanciaAcademica { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public short Cupo { get; set; }

    public bool Activo { get; set; }

    public virtual InstanciaAcademica IdInstanciaAcademicaNavigation { get; set; } = null!;

    public virtual ICollection<MateriaPorGrupo> MateriaPorGrupos { get; set; } = new List<MateriaPorGrupo>();
}
