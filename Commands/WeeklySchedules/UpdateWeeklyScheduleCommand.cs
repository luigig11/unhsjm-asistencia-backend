using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.WeeklySchedules;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.WeeklySchedules
{
    public sealed class UpdateWeeklyScheduleCommand : TransactionalCommandBase<WeeklyScheduleDto>
    {
        private readonly IWeeklyScheduleRepository _repository;
        private Guid _id;
        private UpdateWeeklyScheduleDto? _request;

        public UpdateWeeklyScheduleCommand(IDapperRlsContext dbContext, IWeeklyScheduleRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateWeeklyScheduleCommand WithRequest(Guid id, UpdateWeeklyScheduleDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListWeeklySchedulesCommand.ValidateFields(null, dto.ClassroomId, dto.DayId, dto.StartTime, dto.EndTime);
        }

        protected override async Task<WeeklyScheduleDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                dto.ClassroomId,
                dto.DayId,
                dto.StartTime,
                dto.EndTime,
                dto.IsActive,
                cancellationToken)
                ?? throw new NotFoundApiException($"calendario semanal con id '{_id}' no fue encontrado.");

            return ListWeeklySchedulesCommand.MapToDto(updated);
        }
    }
}