using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<DashboardSchoolPeriodEntity?> GetSchoolPeriodForDashboardAsync(Guid? schoolPeriodId, CancellationToken cancellationToken = default);
        Task<int> CountOfferedGroupsAsync(Guid schoolPeriodId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DashboardTodaySessionEntity>> GetDashboardTodaySessionsAsync(
            Guid schoolPeriodId,
            DateTimeOffset dayStartUtc,
            DateTimeOffset dayEndUtc,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DashboardUpcomingEventEntity>> GetUpcomingEventsAsync(
            Guid schoolPeriodId,
            DateTimeOffset fromUtc,
            int limit = 10,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TeacherTodaySessionEntity>> GetTeacherTodaySessionsAsync(
            Guid teacherId,
            DateTimeOffset dayStartUtc,
            DateTimeOffset dayEndUtc,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TeacherCourseEntity>> GetTeacherCoursesAsync(
            Guid teacherId,
            Guid? academicInstanceId,
            CancellationToken cancellationToken = default);

        Task<AttendanceSheetSessionEntity?> GetSessionSheetSessionAsync(Guid calendarSessionId, CancellationToken cancellationToken = default);
        Task<AttendanceSheetHeaderEntity?> GetSessionSheetHeaderAsync(Guid calendarSessionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AttendanceSheetStudentEntity>> GetSessionSheetStudentsAsync(
            Guid calendarSessionId,
            Guid groupSubjectId,
            CancellationToken cancellationToken = default);

        Task<bool> TeacherExistsAndActiveAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, AbsenceReasonEntity>> GetAbsenceReasonsByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);
        Task<IReadOnlySet<Guid>> GetActiveEnrollmentIdsForGroupSubjectAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default);

        Task<AttendanceEntity> UpsertAttendanceHeaderAsync(
            Guid calendarSessionId,
            bool assignedTeacherAttended,
            Guid? substituteTeacherId,
            Guid? absenceReasonId,
            DateTimeOffset teacherRecordedAtUtc,
            CancellationToken cancellationToken = default);

        Task UpsertAttendanceDetailsAsync(
            Guid attendanceId,
            IReadOnlyList<(Guid GroupEnrollmentId, bool Attended, Guid? AbsenceReasonId)> details,
            DateTimeOffset studentRecordedAtUtc,
            CancellationToken cancellationToken = default);

        Task<AttendanceDetailEntity?> GetAttendanceDetailByIdAsync(Guid detailId, CancellationToken cancellationToken = default);
        Task<AttendanceDetailEntity> UpdateAttendanceDetailAsync(
            Guid detailId,
            bool attended,
            Guid? absenceReasonId,
            DateTimeOffset studentRecordedAtUtc,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAttendanceByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default);

        Task<RosterCourseHeaderEntity?> GetRosterCourseHeaderAsync(Guid groupSubjectId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RosterSessionColumnEntity>> GetRosterSessionsAsync(Guid groupSubjectId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RosterStudentRowEntity>> GetRosterStudentsAsync(Guid groupSubjectId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RosterMarkRowEntity>> GetRosterMarksAsync(Guid groupSubjectId, CancellationToken cancellationToken = default);
    }
}