namespace unhsjm_asistencia_backend.Entities
{
    public sealed class UserEntity
    {
        public Guid Id { get; set; }
        public Guid? AuthUserId { get; set; }
        public Guid RoleId { get; set; }
        public string? NationalId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class UserWithDetailsEntity
    {
        public Guid Id { get; set; }
        public Guid? AuthUserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public bool IsActive { get; set; }

        public Guid? TeacherId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Phone { get; set; }
        public string? ContractType { get; set; }
        public bool? TeacherIsActive { get; set; }
    }
}
