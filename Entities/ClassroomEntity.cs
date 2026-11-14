namespace unhsjm_asistencia_backend.Entities
{
    public sealed class ClassroomEntity
    {
        public Guid Id { get; set; }
        public Guid ClassroomTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class ClassroomWithTypeEntity
    {
        public Guid Id { get; set; }
        public Guid ClassroomTypeId { get; set; }
        public string ClassroomTypeName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }
}
