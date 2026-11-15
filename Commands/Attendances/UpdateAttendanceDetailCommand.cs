using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Attendances
{
    public sealed class UpdateAttendanceDetailCommand : TransactionalCommandBase<AttendanceDetailResponseDto>
    {
        private readonly IAttendanceRepository _repository;
        private Guid _detailId;
        private UpdateAttendanceDetailDto _dto = new();

        public UpdateAttendanceDetailCommand(
            IDapperRlsContext dbContext,
            IAttendanceRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAttendanceDetailCommand WithRequest(Guid detailId, UpdateAttendanceDetailDto dto)
        {
            _detailId = detailId;
            _dto = dto;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var validator = Validator.Create()
                .Field("id", _detailId, "attendance detail")
                .RequiredGuid()
                .Rule(
                    "id_motivo_ausencia",
                    () => !_dto.Attended || !_dto.AbsenceReasonId.HasValue,
                    "An absence reason cannot be provided when the student attended.");

            if (_dto.AbsenceReasonId.HasValue)
            {
                validator.Field("id_motivo_ausencia", _dto.AbsenceReasonId.Value, "absence reason").RequiredGuid();
            }

            validator.ValidateAndThrow();
        }

        protected override async Task<AttendanceDetailResponseDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            _ = await _repository.GetAttendanceDetailByIdAsync(_detailId, cancellationToken)
                ?? throw new NotFoundApiException($"Attendance detail '{_detailId}' was not found.");

            if (_dto.AbsenceReasonId.HasValue)
            {
                var reasonsMap = await _repository.GetAbsenceReasonsByIdsAsync([_dto.AbsenceReasonId.Value], cancellationToken);
                SaveSessionAttendanceSheetCommand.ValidateAbsenceReason(
                    _dto.AbsenceReasonId.Value,
                    "id_motivo_ausencia",
                    reasonsMap,
                    requiredTarget: "ESTUDIANTE");
            }

            var nowUtc = TimeZoneHelper.ToUtc(DateTimeOffset.UtcNow);
            var updated = await _repository.UpdateAttendanceDetailAsync(
                _detailId,
                _dto.Attended,
                _dto.AbsenceReasonId,
                nowUtc,
                cancellationToken);

            return new AttendanceDetailResponseDto
            {
                Id = updated.Id,
                AttendanceId = updated.AttendanceId,
                GroupEnrollmentId = updated.GroupEnrollmentId,
                Attended = updated.Attended,
                AbsenceReasonId = updated.AbsenceReasonId,
                StudentRecordedAt = TimeZoneHelper.ToManagua(updated.StudentRecordedAt)
            };
        }
    }
}