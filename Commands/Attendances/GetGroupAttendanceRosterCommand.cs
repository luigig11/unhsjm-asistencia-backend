using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Attendances;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Attendances
{
    public sealed class GetGroupAttendanceRosterCommand : CommandBase<GroupAttendanceRosterDto>
    {
        private readonly IAttendanceRepository _repository;
        private Guid _groupSubjectId;

        public GetGroupAttendanceRosterCommand(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public GetGroupAttendanceRosterCommand WithGroupSubjectId(Guid groupSubjectId)
        {
            _groupSubjectId = groupSubjectId;
            return this;
        }

        public override async Task<GroupAttendanceRosterDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Validator.Create()
                .Field("id", _groupSubjectId, "group subject")
                .RequiredGuid()
                .ValidateAndThrow();

            var course = await _repository.GetRosterCourseHeaderAsync(_groupSubjectId, cancellationToken)
                ?? throw new NotFoundApiException($"Group subject '{_groupSubjectId}' was not found.");

            var sessions = await _repository.GetRosterSessionsAsync(_groupSubjectId, cancellationToken);
            var students = await _repository.GetRosterStudentsAsync(_groupSubjectId, cancellationToken);
            var marks = await _repository.GetRosterMarksAsync(_groupSubjectId, cancellationToken);

            var marksLookup = marks.ToDictionary(m => (m.GroupEnrollmentId, m.CalendarSessionId));

            var sessionColumns = sessions
                .Select((s, index) => new RosterSessionColumnDto
                {
                    CalendarSessionId = s.CalendarSessionId,
                    MeetingNumber = index + 1,
                    StartDate = TimeZoneHelper.ToManagua(s.StartDate),
                    HasAttendanceRecord = s.HasAttendanceRecord
                })
                .ToList();

            var studentRows = new List<RosterStudentRowDto>(students.Count);

            foreach (var student in students)
            {
                var studentMarks = new List<RosterMarkDto>(sessions.Count);
                var totalPresent = 0;
                var totalUnexcusedAbsent = 0;
                var totalExcused = 0;

                foreach (var session in sessions)
                {
                    if (!session.HasAttendanceRecord
                        || !marksLookup.TryGetValue((student.GroupEnrollmentId, session.CalendarSessionId), out var mark))
                    {
                        studentMarks.Add(new RosterMarkDto
                        {
                            CalendarSessionId = session.CalendarSessionId,
                            Status = "PENDIENTE",
                            AbsenceReasonName = null,
                            StudentRecordedAt = null
                        });
                        continue;
                    }

                    string status;
                    string? reasonName = null;

                    if (mark.Attended)
                    {
                        status = "P";
                        totalPresent++;
                    }
                    else if (mark.AbsenceReasonId.HasValue && mark.JustifiesAbsence)
                    {
                        status = "J";
                        reasonName = mark.AbsenceReasonName;
                        totalExcused++;
                    }
                    else
                    {
                        status = "A";
                        reasonName = mark.AbsenceReasonName;
                        totalUnexcusedAbsent++;
                    }

                    studentMarks.Add(new RosterMarkDto
                    {
                        CalendarSessionId = session.CalendarSessionId,
                        Status = status,
                        AbsenceReasonName = reasonName,
                        StudentRecordedAt = TimeZoneHelper.ToManagua(mark.StudentRecordedAt)
                    });
                }

                var evaluatedSessions = totalPresent + totalUnexcusedAbsent + totalExcused;
                var attendancePercentage = evaluatedSessions == 0
                    ? 100.00m
                    : Math.Round((totalPresent + totalExcused) * 100m / evaluatedSessions, 2, MidpointRounding.AwayFromZero);

                var riskAlert = attendancePercentage switch
                {
                    >= 80m => "VERDE",
                    >= 70m => "AMARILLO",
                    _ => "ROJO"
                };

                studentRows.Add(new RosterStudentRowDto
                {
                    GroupEnrollmentId = student.GroupEnrollmentId,
                    StudentId = student.StudentId,
                    UniqueCode = student.UniqueCode,
                    StudentCarnet = student.StudentCarnet,
                    FullName = student.FullName,
                    Marks = studentMarks,
                    Stats = new RosterStudentStatsDto
                    {
                        TotalPresent = totalPresent,
                        TotalUnexcusedAbsent = totalUnexcusedAbsent,
                        TotalExcused = totalExcused,
                        AttendancePercentage = attendancePercentage,
                        RiskAlert = riskAlert
                    }
                });
            }

            return new GroupAttendanceRosterDto
            {
                Course = new RosterCourseInfoDto
                {
                    GroupSubjectId = course.GroupSubjectId,
                    SubjectCode = course.SubjectCode,
                    SubjectName = course.SubjectName,
                    GroupCode = course.GroupCode,
                    TeacherName = course.TeacherFullName,
                    AcademicInstanceName = course.AcademicInstanceName
                },
                SessionColumns = sessionColumns,
                StudentRows = studentRows
            };
        }
    }
}