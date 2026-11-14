namespace unhsjm_asistencia_backend.Entities
{
    public sealed class WeeklyScheduleEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid ClassroomId { get; set; }
        public short DayId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class WeeklyScheduleWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid ClassroomId { get; set; }
        public string ClassroomCode { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;
        public short DayId { get; set; }
        public string DayName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
