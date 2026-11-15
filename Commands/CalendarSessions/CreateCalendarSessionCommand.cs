using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CalendarSessions
{
    public sealed class CreateCalendarSessionCommand : TransactionalCommandBase<CalendarSessionDto>
    {
        private readonly ICalendarSessionRepository _repository;
        private CreateCalendarSessionDto? _request;

        public CreateCalendarSessionCommand(IDapperRlsContext dbContext, ICalendarSessionRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateCalendarSessionCommand WithRequest(CreateCalendarSessionDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListCalendarSessionsCommand.ValidateFields(dto.GroupSubjectId, dto.ClassroomId, dto.StartDate, dto.EndDate);
        }

        protected override async Task<CalendarSessionDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new CalendarSessionEntity
                {
                    GroupSubjectId = dto.GroupSubjectId,
                    ClassroomId = dto.ClassroomId,
                    StartDate = TimeZoneHelper.ToUtc(dto.StartDate),
                    EndDate = TimeZoneHelper.ToUtc(dto.EndDate),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListCalendarSessionsCommand.MapToDto(created);
        }
    }
}