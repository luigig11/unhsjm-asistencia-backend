namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AbsenceReasonEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool JustifiesAbsence { get; set; }
        public string AppliesTo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
