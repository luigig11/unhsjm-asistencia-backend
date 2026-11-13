using System;
using System.Collections.Generic;

namespace unhsjm_asistencia_backend.Models;

public partial class MallaCurricular
{
    public Guid Id { get; set; }

    public Guid IdDetallePlanAcademico { get; set; }

    public Guid IdMateria { get; set; }

    public short EncuentrosTotales { get; set; }

    public short EncuentrosPorSemana { get; set; }

    public decimal CantidadHorasMateria { get; set; }

    public short Creditos { get; set; }

    public bool Obligatoria { get; set; }

    public bool Activo { get; set; }

    public virtual DetallePlanAcademico IdDetallePlanAcademicoNavigation { get; set; } = null!;

    public virtual Materium IdMateriaNavigation { get; set; } = null!;

    public virtual ICollection<MateriaPorGrupo> MateriaPorGrupos { get; set; } = new List<MateriaPorGrupo>();

    public virtual ICollection<MateriaPrerequisito> MateriaPrerequisitoIdMallaCurricularMateriaNavigations { get; set; } = new List<MateriaPrerequisito>();

    public virtual ICollection<MateriaPrerequisito> MateriaPrerequisitoIdMallaCurricularMateriaPrerequisitoNavigations { get; set; } = new List<MateriaPrerequisito>();
}
