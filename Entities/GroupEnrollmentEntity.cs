namespace unhsjm_asistencia_backend.Entities
{
    public sealed class GroupEnrollmentEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid StudentId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class GroupEnrollmentWithStudentEntity
    {
        public Guid Id { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid StudentId { get; set; }
        public string UniqueCode { get; set; } = string.Empty;
        public string StudentCarnet { get; set; } = string.Empty;
        public string FirstNames { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }
}
