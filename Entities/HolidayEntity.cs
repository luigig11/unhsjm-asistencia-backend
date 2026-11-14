namespace unhsjm_asistencia_backend.Entities
{
    public sealed class HolidayEntity
    {
        public Guid Id { get; set; }
        public Guid SchoolPeriodId { get; set; }
        public Guid EventTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class HolidayWithEventTypeEntity
    {
        public Guid Id { get; set; }
        public Guid SchoolPeriodId { get; set; }
        public Guid EventTypeId { get; set; }
        public string EventTypeCode { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
