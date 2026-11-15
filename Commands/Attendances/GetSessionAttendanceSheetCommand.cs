using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Attendances
{
    public sealed class GetSessionAttendanceSheetCommand : CommandBase<SessionAttendanceSheetDto>
    {
        private readonly IAttendanceRepository _repository;
        private Guid _calendarSessionId;

        public GetSessionAttendanceSheetCommand(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public GetSessionAttendanceSheetCommand WithCalendarSessionId(Guid calendarSessionId)
        {
            _calendarSessionId = calendarSessionId;
            return this;
        }

        public override async Task<SessionAttendanceSheetDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Validator.Create()
                .Field("idCalendario", _calendarSessionId, "calendar session")
                .RequiredGuid()
                .ValidateAndThrow();

            var session = await _repository.GetSessionSheetSessionAsync(_calendarSessionId, cancellationToken)
                ?? throw new NotFoundApiException($"Calendar session '{_calendarSessionId}' was not found.");

            var header = await _repository.GetSessionSheetHeaderAsync(_calendarSessionId, cancellationToken);
            var students = await _repository.GetSessionSheetStudentsAsync(_calendarSessionId, session.GroupSubjectId, cancellationToken);

            return new SessionAttendanceSheetDto
            {
                Session = new SessionAttendanceInfoDto
                {
                    CalendarSessionId = session.CalendarSessionId,
                    GroupSubjectId = session.GroupSubjectId,
                    SubjectCode = session.SubjectCode,
                    SubjectName = session.SubjectName,
                    GroupCode = session.GroupCode,
                    ClassroomCode = session.ClassroomCode,
                    TitularTeacherName = session.TitularTeacherFullName,
                    StartDate = TimeZoneHelper.ToManagua(session.StartDate),
                    EndDate = TimeZoneHelper.ToManagua(session.EndDate)
                },
                AttendanceHeader = header is null
                    ? new SessionAttendanceHeaderDto()
                    : new SessionAttendanceHeaderDto
                    {
                        Id = header.Id,
                        AssignedTeacherAttended = header.AssignedTeacherAttended,
                        SubstituteTeacherId = header.SubstituteTeacherId,
                        SubstituteTeacherName = header.SubstituteTeacherFullName,
                        AbsenceReasonId = header.AbsenceReasonId,
                        TeacherRecordedAt = TimeZoneHelper.ToManagua(header.TeacherRecordedAt)
                    },
                Students = students
                    .Select(s => new SessionAttendanceStudentDto
                    {
                        AttendanceDetailId = s.AttendanceDetailId,
                        GroupEnrollmentId = s.GroupEnrollmentId,
                        StudentId = s.StudentId,
                        UniqueCode = s.UniqueCode,
                        StudentCarnet = s.StudentCarnet,
                        FullName = s.FullName,
                        Attended = s.Attended,
                        AbsenceReasonId = s.AbsenceReasonId,
                        StudentRecordedAt = s.StudentRecordedAt.HasValue
                            ? TimeZoneHelper.ToManagua(s.StudentRecordedAt.Value)
                            : null
                    })
                    .ToList()
            };
        }
    }
}