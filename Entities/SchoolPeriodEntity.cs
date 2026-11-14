namespace unhsjm_asistencia_backend.Entities
{
    public sealed class SchoolPeriodEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
