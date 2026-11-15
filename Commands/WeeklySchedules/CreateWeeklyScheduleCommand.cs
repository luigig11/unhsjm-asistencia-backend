using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.WeeklySchedules;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.WeeklySchedules
{
    public sealed class CreateWeeklyScheduleCommand : TransactionalCommandBase<WeeklyScheduleDto>
    {
        private readonly IWeeklyScheduleRepository _repository;
        private CreateWeeklyScheduleDto? _request;

        public CreateWeeklyScheduleCommand(IDapperRlsContext dbContext, IWeeklyScheduleRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateWeeklyScheduleCommand WithRequest(CreateWeeklyScheduleDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListWeeklySchedulesCommand.ValidateFields(dto.GroupSubjectId, dto.ClassroomId, dto.DayId, dto.StartTime, dto.EndTime);
        }

        protected override async Task<WeeklyScheduleDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new WeeklyScheduleEntity
                {
                    GroupSubjectId = dto.GroupSubjectId,
                    ClassroomId = dto.ClassroomId,
                    DayId = dto.DayId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListWeeklySchedulesCommand.MapToDto(created);
        }
    }
}