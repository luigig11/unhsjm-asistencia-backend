namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AcademicPeriodTypeEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public short MinMonthsDuration { get; set; }
        public short MaxMonthsDuration { get; set; }
        public short MinHoursDuration { get; set; }
        public short MaxHoursDuration { get; set; }
        public bool IsActive { get; set; }
    }
}
