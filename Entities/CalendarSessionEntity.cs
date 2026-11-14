namespace unhsjm_asistencia_backend.Entities
{
    public sealed class CalendarSessionEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid ClassroomId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class CalendarSessionWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid ClassroomId { get; set; }
        public string ClassroomCode { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }
        public string AttendanceStatus { get; set; } = "PENDIENTE";
    }
}
