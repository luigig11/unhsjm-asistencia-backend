namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AcademicInstanceEntity
    {
        public Guid Id { get; set; }
        public Guid SchoolPeriodId { get; set; }
        public Guid AcademicPlanDetailId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AcademicInstanceWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid SchoolPeriodId { get; set; }
        public string SchoolPeriodName { get; set; } = string.Empty;
        public Guid AcademicPlanDetailId { get; set; }
        public string AcademicPlanCode { get; set; } = string.Empty;
        public string AcademicCycleName { get; set; } = string.Empty;
        public string AcademicPeriodName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }

}
