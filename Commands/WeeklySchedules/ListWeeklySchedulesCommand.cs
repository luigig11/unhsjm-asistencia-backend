using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.WeeklySchedules;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.WeeklySchedules
{
    public sealed class ListWeeklySchedulesCommand : CommandBase<IReadOnlyList<WeeklyScheduleListItemDto>>
    {
        private readonly IWeeklyScheduleRepository _repository;
        private Guid? _groupSubjectId;
        private bool? _isActive;

        public ListWeeklySchedulesCommand(IWeeklyScheduleRepository repository)
        {
            _repository = repository;
        }

        public ListWeeklySchedulesCommand WithFilter(Guid? groupSubjectId, bool? isActive)
        {
            _groupSubjectId = groupSubjectId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<WeeklyScheduleListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_groupSubjectId, _isActive, cancellationToken);
            return items
                .Select(entity => new WeeklyScheduleListItemDto
                {
                    Id = entity.Id,
                    GroupSubjectId = entity.GroupSubjectId,
                    ClassroomId = entity.ClassroomId,
                    ClassroomCode = entity.ClassroomCode,
                    ClassroomName = entity.ClassroomName,
                    DayId = entity.DayId,
                    DayName = entity.DayName,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static WeeklyScheduleDto MapToDto(WeeklyScheduleEntity entity) => new()
        {
            Id = entity.Id,
            GroupSubjectId = entity.GroupSubjectId,
            ClassroomId = entity.ClassroomId,
            DayId = entity.DayId,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid? groupSubjectId,
            Guid classroomId,
            short dayId,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            var validator = Validator.Create();

            if (groupSubjectId.HasValue)
            {
                validator.Field("id_materia_por_grupo", groupSubjectId.Value, "grupo de materia").RequiredGuid();
            }

            validator
                .Field("id_aula", classroomId, "aula de clase").RequiredGuid()
                .Field("id_dia", dayId, "Dia").Between(1, 7)
                .Rule("hora_fin", () => endTime > startTime, "Hora final debe ser mayor que hora inicial.")
                .ValidateAndThrow();
        }
    }
}