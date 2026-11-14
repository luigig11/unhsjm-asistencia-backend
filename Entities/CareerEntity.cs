namespace unhsjm_asistencia_backend.Entities
{
    public sealed class CareerEntity
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public sealed class CareerWithDepartmentEntity
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
