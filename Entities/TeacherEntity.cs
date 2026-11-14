namespace unhsjm_asistencia_backend.Entities
{
    public sealed class TeacherEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public sealed class TeacherWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
