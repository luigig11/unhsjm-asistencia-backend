namespace unhsjm_asistencia_backend.Entities
{
    public sealed class GroupSubjectEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid CurricularGridItemId { get; set; }
        public Guid TeacherId { get; set; }
        public short QuotaLimit { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class GroupSubjectWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public Guid CurricularGridItemId { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherFullName { get; set; } = string.Empty;
        public short QuotaLimit { get; set; }
        public int TotalEnrolled { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class GroupSubjectGenerationContextEntity
    {
        public Guid GroupSubjectId { get; set; }
        public Guid SchoolPeriodId { get; set; }
        public DateOnly InstanceStartDate { get; set; }
        public DateOnly InstanceEndDate { get; set; }
    }
}
