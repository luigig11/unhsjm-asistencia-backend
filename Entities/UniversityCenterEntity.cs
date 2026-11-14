namespace unhsjm_asistencia_backend.Entities
{
    public sealed class UniversityCenterEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }
}
