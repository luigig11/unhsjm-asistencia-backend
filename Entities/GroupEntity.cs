namespace unhsjm_asistencia_backend.Entities
{
    public sealed class GroupEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicInstanceId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Capacity { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class GroupWithInstanceEntity
    {
        public Guid Id { get; set; }
        public Guid AcademicInstanceId { get; set; }
        public string InstanceName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
