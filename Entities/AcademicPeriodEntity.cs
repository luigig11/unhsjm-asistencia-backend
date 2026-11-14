namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AcademicPeriodEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPeriodTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Order { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AcademicPeriodWithTypeEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPeriodTypeId { get; set; }
        public string AcademicPeriodTypeName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Order { get; set; }
        public bool IsActive { get; set; }
    }
}
