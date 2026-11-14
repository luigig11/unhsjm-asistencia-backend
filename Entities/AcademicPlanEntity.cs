namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AcademicPlanEntity
    {
        public Guid Id { get; set; }
        public Guid UniversityCenterId { get; set; }
        public Guid CareerId { get; set; }
        public Guid AcademicPeriodTypeId { get; set; }
        public Guid ModalityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short AcademicCycleCount { get; set; }
        public short AcademicPeriodCount { get; set; }
        public DateOnly ActiveFrom { get; set; }
        public DateOnly? ActiveUntil { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AcademicPlanWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid UniversityCenterId { get; set; }
        public string UniversityCenterName { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public Guid CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public Guid AcademicPeriodTypeId { get; set; }
        public string AcademicPeriodTypeName { get; set; } = string.Empty;
        public Guid ModalityId { get; set; }
        public string ModalityName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short AcademicCycleCount { get; set; }
        public short AcademicPeriodCount { get; set; }
        public DateOnly ActiveFrom { get; set; }
        public DateOnly? ActiveUntil { get; set; }
        public bool IsActive { get; set; }
    }
}
