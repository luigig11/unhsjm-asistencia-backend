namespace unhsjm_asistencia_backend.Entities
{
    public sealed class StudentEntity
    {
        public Guid Id { get; set; }
        public string UniqueCode { get; set; } = string.Empty;
        public Guid CareerId { get; set; }
        public Guid AcademicPlanId { get; set; }
        public string StudentCarnet { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string FirstNames { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly AdmissionDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class StudentWithDetailsEntity
    {
        public Guid Id { get; set; }
        public string UniqueCode { get; set; } = string.Empty;
        public string StudentCarnet { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string FirstNames { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly AdmissionDate { get; set; }
        public Guid CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public Guid AcademicPlanId { get; set; }
        public string AcademicPlanCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
