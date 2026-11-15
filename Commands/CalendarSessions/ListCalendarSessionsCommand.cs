using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CalendarSessions
{
    public sealed class ListCalendarSessionsCommand : CommandBase<IReadOnlyList<CalendarSessionListItemDto>>
    {
        private readonly ICalendarSessionRepository _repository;
        private Guid? _groupSubjectId;
        private DateOnly? _date;
        private bool? _isActive;

        public ListCalendarSessionsCommand(ICalendarSessionRepository repository)
        {
            _repository = repository;
        }

        public ListCalendarSessionsCommand WithFilter(Guid? groupSubjectId, DateOnly? date, bool? isActive)
        {
            _groupSubjectId = groupSubjectId;
            _date = date;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<CalendarSessionListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            DateTimeOffset? dayStartUtc = null;
            DateTimeOffset? dayEndUtc = null;

            if (_date.HasValue)
            {
                (dayStartUtc, dayEndUtc) = TimeZoneHelper.GetManaguaDayRangeUtc(_date.Value);
            }

            var items = await _repository.GetAllAsync(_groupSubjectId, dayStartUtc, dayEndUtc, _isActive, cancellationToken);
            return items
                .Select(entity => new CalendarSessionListItemDto
                {
                    Id = entity.Id,
                    GroupSubjectId = entity.GroupSubjectId,
                    ClassroomId = entity.ClassroomId,
                    ClassroomCode = entity.ClassroomCode,
                    StartDate = TimeZoneHelper.ToManagua(entity.StartDate),
                    EndDate = TimeZoneHelper.ToManagua(entity.EndDate),
                    IsActive = entity.IsActive,
                    AttendanceStatus = entity.AttendanceStatus
                })
                .ToList();
        }

        internal static CalendarSessionDto MapToDto(CalendarSessionEntity entity) => new()
        {
            Id = entity.Id,
            GroupSubjectId = entity.GroupSubjectId,
            ClassroomId = entity.ClassroomId,
            StartDate = TimeZoneHelper.ToManagua(entity.StartDate),
            EndDate = TimeZoneHelper.ToManagua(entity.EndDate),
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid? groupSubjectId,
            Guid classroomId,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            var validator = Validator.Create();

            if (groupSubjectId.HasValue)
            {
                validator.Field("id_materia_por_grupo", groupSubjectId.Value, "materia por grupo").RequiredGuid();
            }

            validator
                .Field("id_aula", classroomId, "classroom").RequiredGuid()
                .Rule("fecha_fin", () => endDate > startDate, "Fecha de fin debe ser mayor que fecha de inicio.")
                .ValidateAndThrow();
        }
    }
}