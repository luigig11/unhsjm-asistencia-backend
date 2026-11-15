using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CalendarSessions
{
    public sealed class UpdateCalendarSessionCommand : TransactionalCommandBase<CalendarSessionDto>
    {
        private readonly ICalendarSessionRepository _repository;
        private Guid _id;
        private UpdateCalendarSessionDto? _request;

        public UpdateCalendarSessionCommand(IDapperRlsContext dbContext, ICalendarSessionRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateCalendarSessionCommand WithRequest(Guid id, UpdateCalendarSessionDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListCalendarSessionsCommand.ValidateFields(null, dto.ClassroomId, dto.StartDate, dto.EndDate);
        }

        protected override async Task<CalendarSessionDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                dto.ClassroomId,
                TimeZoneHelper.ToUtc(dto.StartDate),
                TimeZoneHelper.ToUtc(dto.EndDate),
                dto.IsActive,
                cancellationToken)
                ?? throw new NotFoundApiException($"Sesion calendario con id '{_id}' no fue encontrado.");

            return ListCalendarSessionsCommand.MapToDto(updated);
        }
    }
}