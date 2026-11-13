using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class MateriaPrerequisito
{
    public Guid Id { get; set; }

    public Guid IdMallaCurricularMateria { get; set; }

    public Guid IdMallaCurricularMateriaPrerequisito { get; set; }

    public bool Activo { get; set; }

    public virtual MallaCurricular IdMallaCurricularMateriaNavigation { get; set; } = null!;

    public virtual MallaCurricular IdMallaCurricularMateriaPrerequisitoNavigation { get; set; } = null!;
}
