namespace unhsjm_asistencia_backend.Entities
{
    public sealed class AttendanceEntity
    {
        public Guid Id { get; set; }
        public Guid CalendarSessionId { get; set; }
        public Guid? SubstituteTeacherId { get; set; }
        public bool? AssignedTeacherAttended { get; set; }
        public Guid? AbsenceReasonId { get; set; }
        public DateTimeOffset TeacherRecordedAt { get; set; }
    }

    public sealed class AttendanceDetailEntity
    {
        public Guid Id { get; set; }
        public Guid AttendanceId { get; set; }
        public Guid GroupEnrollmentId { get; set; }
        public bool Attended { get; set; }
        public Guid? AbsenceReasonId { get; set; }
        public DateTimeOffset StudentRecordedAt { get; set; }
    }

    public sealed class DashboardSchoolPeriodEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class DashboardTodaySessionEntity
    {
        public Guid CalendarSessionId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty;
    }

    public sealed class DashboardUpcomingEventEntity
    {
        public Guid HolidayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    public sealed class TeacherTodaySessionEntity
    {
        public Guid CalendarSessionId { get; set; }
        public Guid GroupSubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = string.Empty;
        public string? ClassroomLocation { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Guid? AttendanceId { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsSubstitute { get; set; }
    }

    public sealed class TeacherCourseEntity
    {
        public Guid GroupSubjectId { get; set; }
        public string AcademicInstanceName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int TotalEnrolled { get; set; }
        public short QuotaLimit { get; set; }
    }

    public sealed class AttendanceSheetSessionEntity
    {
        public Guid CalendarSessionId { get; set; }
        public Guid GroupSubjectId { get; set; }
        public Guid TitularTeacherId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = string.Empty;
        public string TitularTeacherFullName { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    public sealed class AttendanceSheetHeaderEntity
    {
        public Guid Id { get; set; }
        public bool? AssignedTeacherAttended { get; set; }
        public Guid? SubstituteTeacherId { get; set; }
        public string? SubstituteTeacherFullName { get; set; }
        public Guid? AbsenceReasonId { get; set; }
        public DateTimeOffset TeacherRecordedAt { get; set; }
    }

    public sealed class AttendanceSheetStudentEntity
    {
        public Guid? AttendanceDetailId { get; set; }
        public Guid GroupEnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string UniqueCode { get; set; } = string.Empty;
        public string StudentCarnet { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool Attended { get; set; }
        public Guid? AbsenceReasonId { get; set; }
        public DateTimeOffset? StudentRecordedAt { get; set; }
    }

    public sealed class RosterCourseHeaderEntity
    {
        public Guid GroupSubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public string AcademicInstanceName { get; set; } = string.Empty;
    }

    public sealed class RosterSessionColumnEntity
    {
        public Guid CalendarSessionId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public bool HasAttendanceRecord { get; set; }
    }

    public sealed class RosterStudentRowEntity
    {
        public Guid GroupEnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string UniqueCode { get; set; } = string.Empty;
        public string StudentCarnet { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public sealed class RosterMarkRowEntity
    {
        public Guid GroupEnrollmentId { get; set; }
        public Guid CalendarSessionId { get; set; }
        public bool Attended { get; set; }
        public Guid? AbsenceReasonId { get; set; }
        public string? AbsenceReasonName { get; set; }
        public bool JustifiesAbsence { get; set; }
        public DateTimeOffset StudentRecordedAt { get; set; }
    }
}
