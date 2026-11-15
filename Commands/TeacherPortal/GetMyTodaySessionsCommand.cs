using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.TeacherPortal;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.TeacherPortal
{
    public sealed class GetMyTodaySessionsCommand : CommandBase<IReadOnlyList<TeacherTodaySessionDto>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyTodaySessionsCommand(
            IAttendanceRepository attendanceRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<IReadOnlyList<TeacherTodaySessionDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var teacherId = await TeacherIdentityResolver.ResolveAuthenticatedTeacherIdAsync(
                _httpContextAccessor,
                _userRepository,
                cancellationToken);

            var todayManagua = TimeZoneHelper.TodayInManagua();
            var (dayStartUtc, dayEndUtc) = TimeZoneHelper.GetManaguaDayRangeUtc(todayManagua);

            var items = await _attendanceRepository.GetTeacherTodaySessionsAsync(
                teacherId,
                dayStartUtc,
                dayEndUtc,
                cancellationToken);

            return items
                .Select(s => new TeacherTodaySessionDto
                {
                    CalendarSessionId = s.CalendarSessionId,
                    GroupSubjectId = s.GroupSubjectId,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    GroupCode = s.GroupCode,
                    ClassroomCode = s.ClassroomCode,
                    ClassroomLocation = s.ClassroomLocation,
                    StartDate = TimeZoneHelper.ToManagua(s.StartDate),
                    EndDate = TimeZoneHelper.ToManagua(s.EndDate),
                    AttendanceId = s.AttendanceId,
                    IsRegistered = s.IsRegistered,
                    IsSubstitute = s.IsSubstitute
                })
                .ToList();
        }
    }
}