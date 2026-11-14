namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AcademicPlanDetailEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPlanId { get; set; }
        public Guid AcademicCycleId { get; set; }
        public Guid AcademicPeriodId { get; set; }
        public short DurationWeeks { get; set; }
        public short DurationMonths { get; set; }
        public Guid? PredecessorDetailId { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AcademicPlanDetailWithNamesEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPlanId { get; set; }
        public Guid AcademicCycleId { get; set; }
        public string AcademicCycleCode { get; set; } = string.Empty;
        public string AcademicCycleName { get; set; } = string.Empty;
        public short AcademicCycleOrder { get; set; }
        public Guid AcademicPeriodId { get; set; }
        public string AcademicPeriodCode { get; set; } = string.Empty;
        public string AcademicPeriodName { get; set; } = string.Empty;
        public short AcademicPeriodOrder { get; set; }
        public short DurationWeeks { get; set; }
        public short DurationMonths { get; set; }
        public Guid? PredecessorDetailId { get; set; }
        public bool IsActive { get; set; }
    }
}
