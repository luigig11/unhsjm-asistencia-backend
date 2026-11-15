using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Holidays;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Holidays
{
    public sealed class ListHolidaysCommand : CommandBase<IReadOnlyList<HolidayListItemDto>>
    {
        private readonly IHolidayRepository _repository;
        private Guid? _schoolPeriodId;
        private Guid? _eventTypeId;
        private bool? _isActive;

        public ListHolidaysCommand(IHolidayRepository repository)
        {
            _repository = repository;
        }

        public ListHolidaysCommand WithFilter(Guid? schoolPeriodId, Guid? eventTypeId, bool? isActive)
        {
            _schoolPeriodId = schoolPeriodId;
            _eventTypeId = eventTypeId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<HolidayListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_schoolPeriodId, _eventTypeId, _isActive, cancellationToken);
            return items
                .Select(entity => new HolidayListItemDto
                {
                    Id = entity.Id,
                    SchoolPeriodId = entity.SchoolPeriodId,
                    EventTypeId = entity.EventTypeId,
                    EventTypeCode = entity.EventTypeCode,
                    EventTypeName = entity.EventTypeName,
                    Name = entity.Name,
                    StartDate = TimeZoneHelper.ToManagua(entity.StartDate),
                    EndDate = TimeZoneHelper.ToManagua(entity.EndDate),
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static HolidayDto MapToDto(HolidayEntity entity) => new()
        {
            Id = entity.Id,
            SchoolPeriodId = entity.SchoolPeriodId,
            EventTypeId = entity.EventTypeId,
            Name = entity.Name,
            StartDate = TimeZoneHelper.ToManagua(entity.StartDate),
            EndDate = TimeZoneHelper.ToManagua(entity.EndDate),
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid schoolPeriodId,
            Guid eventTypeId,
            string? name,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            Validator.Create()
                .Field("id_periodo_escolar", schoolPeriodId, "periodo escolar").RequiredGuid()
                .Field("id_tipo_evento", eventTypeId, "tipo de evento").RequiredGuid()
                .Field("nombre", name, "nombre").NotEmpty().MaxLength(150)
                .Rule("fecha_fin", () => endDate >= startDate, "fecha de fin debe ser mayor o igual que fecha de inicio.")
                .ValidateAndThrow();
        }
    }
}