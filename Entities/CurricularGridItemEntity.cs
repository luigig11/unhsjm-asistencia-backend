namespace unhsjm_asistencia_backend.Entities
{
    public sealed class CurricularGridItemEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPlanDetailId { get; set; }
        public Guid SubjectId { get; set; }
        public short TotalMeetings { get; set; }
        public short MeetingsPerWeek { get; set; }
        public decimal SubjectHours { get; set; }
        public short Credits { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class CurricularGridItemWithSubjectEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicPlanDetailId { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public short TotalMeetings { get; set; }
        public short MeetingsPerWeek { get; set; }
        public decimal SubjectHours { get; set; }
        public short Credits { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}
