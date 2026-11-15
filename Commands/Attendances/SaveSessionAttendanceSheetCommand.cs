using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Attendances
{
    public sealed class SaveSessionAttendanceSheetCommand : TransactionalCommandBase<SaveAttendanceSheetResponseDto>
    {
        private readonly IAttendanceRepository _repository;
        private Guid _calendarSessionId;
        private SaveAttendanceSheetDto _dto = new();

        public SaveSessionAttendanceSheetCommand(
            IDapperRlsContext dbContext,
            IAttendanceRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public SaveSessionAttendanceSheetCommand WithRequest(Guid calendarSessionId, SaveAttendanceSheetDto dto)
        {
            _calendarSessionId = calendarSessionId;
            _dto = dto;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var details = _dto.Details ?? [];

            var validator = Validator.Create()
                .Field("idCalendario", _calendarSessionId, "calendar session")
                .RequiredGuid()
                .Rule(
                    "id_docente_sustituto",
                    () => !(_dto.AssignedTeacherAttended && _dto.SubstituteTeacherId.HasValue),
                    "A substitute teacher cannot be assigned when the assigned teacher attended.")
                .Rule(
                    "id_motivo_ausencia",
                    () => !_dto.AssignedTeacherAttended || !_dto.AbsenceReasonId.HasValue,
                    "An absence reason cannot be provided when the assigned teacher attended.")
                .Rule(
                    "detalles",
                    () => details.Count > 0,
                    "At least one student attendance detail item is required.")
                .Rule(
                    "detalles",
                    () => details.Select(d => d.GroupEnrollmentId).Distinct().Count() == details.Count,
                    "Duplicate group enrollment IDs are not allowed in attendance details.");

            if (_dto.SubstituteTeacherId.HasValue)
            {
                validator.Field("id_docente_sustituto", _dto.SubstituteTeacherId.Value, "substitute teacher").RequiredGuid();
            }

            if (_dto.AbsenceReasonId.HasValue)
            {
                validator.Field("id_motivo_ausencia", _dto.AbsenceReasonId.Value, "absence reason").RequiredGuid();
            }

            foreach (var item in details)
            {
                validator
                    .Field("detalles.id_inscripcion_grupo", item.GroupEnrollmentId, "group enrollment")
                    .RequiredGuid()
                    .Rule(
                        "detalles.id_motivo_ausencia",
                        () => !item.Attended || !item.AbsenceReasonId.HasValue,
                        "A student absence reason cannot be provided when the student attended.");

                if (item.AbsenceReasonId.HasValue)
                {
                    validator.Field("detalles.id_motivo_ausencia", item.AbsenceReasonId.Value, "student absence reason").RequiredGuid();
                }
            }

            validator.ValidateAndThrow();
        }

        protected override async Task<SaveAttendanceSheetResponseDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var session = await _repository.GetSessionSheetSessionAsync(_calendarSessionId, cancellationToken)
                ?? throw new NotFoundApiException($"Calendar session '{_calendarSessionId}' was not found.");

            if (_dto.SubstituteTeacherId.HasValue)
            {
                var subTeacherId = _dto.SubstituteTeacherId.Value;
                if (subTeacherId == session.TitularTeacherId)
                {
                    throw new ValidationApiException("One or more validation errors occurred.", new Dictionary<string, string[]>
                    {
                        ["id_docente_sustituto"] = ["The substitute teacher must be different from the titular teacher."]
                    });
                }

                if (!await _repository.TeacherExistsAndActiveAsync(subTeacherId, cancellationToken))
                {
                    throw new ValidationApiException("One or more validation errors occurred.", new Dictionary<string, string[]>
                    {
                        ["id_docente_sustituto"] = [$"Substitute teacher '{subTeacherId}' was not found or is inactive."]
                    });
                }
            }

            var absenceReasonIds = new List<Guid>();
            if (_dto.AbsenceReasonId.HasValue)
            {
                absenceReasonIds.Add(_dto.AbsenceReasonId.Value);
            }

            foreach (var detail in _dto.Details)
            {
                if (detail.AbsenceReasonId.HasValue)
                {
                    absenceReasonIds.Add(detail.AbsenceReasonId.Value);
                }
            }

            var reasonsMap = await _repository.GetAbsenceReasonsByIdsAsync(absenceReasonIds, cancellationToken);

            if (_dto.AbsenceReasonId.HasValue)
            {
                ValidateAbsenceReason(
                    _dto.AbsenceReasonId.Value,
                    "id_motivo_ausencia",
                    reasonsMap,
                    requiredTarget: "DOCENTE");
            }

            foreach (var detail in _dto.Details)
            {
                if (detail.AbsenceReasonId.HasValue)
                {
                    ValidateAbsenceReason(
                        detail.AbsenceReasonId.Value,
                        "detalles.id_motivo_ausencia",
                        reasonsMap,
                        requiredTarget: "ESTUDIANTE");
                }
            }

            var activeEnrollments = await _repository.GetActiveEnrollmentIdsForGroupSubjectAsync(
                session.GroupSubjectId,
                cancellationToken);

            var invalidEnrollmentIds = _dto.Details
                .Select(d => d.GroupEnrollmentId)
                .Where(id => !activeEnrollments.Contains(id))
                .Distinct()
                .ToList();

            if (invalidEnrollmentIds.Count > 0)
            {
                throw new ValidationApiException("One or more validation errors occurred.", new Dictionary<string, string[]>
                {
                    ["detalles"] = [$"One or more group enrollments are inactive or do not belong to this course: {string.Join(", ", invalidEnrollmentIds)}."]
                });
            }

            var nowUtc = TimeZoneHelper.ToUtc(DateTimeOffset.UtcNow);

            var header = await _repository.UpsertAttendanceHeaderAsync(
                _calendarSessionId,
                _dto.AssignedTeacherAttended,
                _dto.SubstituteTeacherId,
                _dto.AbsenceReasonId,
                nowUtc,
                cancellationToken);

            var detailTuples = _dto.Details
                .Select(d => (d.GroupEnrollmentId, d.Attended, d.AbsenceReasonId))
                .ToList();

            await _repository.UpsertAttendanceDetailsAsync(
                header.Id,
                detailTuples,
                nowUtc,
                cancellationToken);

            var presentCount = _dto.Details.Count(d => d.Attended);
            var excusedCount = _dto.Details.Count(d =>
                !d.Attended
                && d.AbsenceReasonId.HasValue
                && reasonsMap.TryGetValue(d.AbsenceReasonId.Value, out var reason)
                && reason.JustifiesAbsence);
            var absentCount = _dto.Details.Count - presentCount - excusedCount;

            return new SaveAttendanceSheetResponseDto
            {
                AttendanceId = header.Id,
                CalendarSessionId = header.CalendarSessionId,
                AssignedTeacherAttended = header.AssignedTeacherAttended ?? _dto.AssignedTeacherAttended,
                SubstituteTeacherId = header.SubstituteTeacherId,
                AbsenceReasonId = header.AbsenceReasonId,
                TeacherRecordedAt = TimeZoneHelper.ToManagua(header.TeacherRecordedAt),
                Summary = new AttendanceSummaryCountsDto
                {
                    PresentCount = presentCount,
                    AbsentCount = absentCount,
                    ExcusedCount = excusedCount
                }
            };
        }

        internal static void ValidateAbsenceReason(
            Guid absenceReasonId,
            string fieldKey,
            IReadOnlyDictionary<Guid, AbsenceReasonEntity> reasonsMap,
            string requiredTarget)
        {
            if (!reasonsMap.TryGetValue(absenceReasonId, out var reason) || !reason.IsActive)
            {
                throw new ValidationApiException("One or more validation errors occurred.", new Dictionary<string, string[]>
                {
                    [fieldKey] = [$"Absence reason '{absenceReasonId}' was not found or is inactive."]
                });
            }

            if (!string.Equals(reason.AppliesTo, requiredTarget, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(reason.AppliesTo, "AMBOS", StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationApiException("One or more validation errors occurred.", new Dictionary<string, string[]>
                {
                    [fieldKey] = [$"Absence reason '{reason.Name}' does not apply to {requiredTarget}."]
                });
            }
        }
    }
}