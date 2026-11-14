namespace unhsjm_asistencia_backend.Entities
{
    public sealed class SubjectPrerequisiteEntity
    {
        public Guid Id { get; set; }
        public Guid CurricularGridItemId { get; set; }
        public Guid PrerequisiteCurricularGridItemId { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class SubjectPrerequisiteWithSubjectEntity
    {
        public Guid Id { get; set; }
        public Guid CurricularGridItemId { get; set; }
        public Guid PrerequisiteCurricularGridItemId { get; set; }
        public Guid PrerequisiteSubjectId { get; set; }
        public string PrerequisiteSubjectCode { get; set; } = string.Empty;
        public string PrerequisiteSubjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
