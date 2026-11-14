namespace unhsjm_asistencia_backend.Entities
{
    public sealed class PermissionEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string[] AllowedOperations { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
