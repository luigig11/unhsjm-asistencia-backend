using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Dashboard;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Dashboard
{
    public sealed class GetDashboardSummaryCommand : CommandBase<DashboardSummaryResponseDto>
    {
        private readonly IAttendanceRepository _repository;
        private Guid? _schoolPeriodId;

        public GetDashboardSummaryCommand(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public GetDashboardSummaryCommand WithSchoolPeriodId(Guid? schoolPeriodId)
        {
            _schoolPeriodId = schoolPeriodId;
            return this;
        }

        public override async Task<DashboardSummaryResponseDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (_schoolPeriodId.HasValue)
            {
                Validator.Create()
                    .Field("id_periodo_escolar", _schoolPeriodId.Value, "school period")
                    .RequiredGuid()
                    .ValidateAndThrow();
            }

            var period = await _repository.GetSchoolPeriodForDashboardAsync(_schoolPeriodId, cancellationToken);
            if (period is null)
            {
                throw new NotFoundApiException(
                    _schoolPeriodId.HasValue
                        ? $"School period '{_schoolPeriodId.Value}' was not found."
                        : "No active school period found.");
            }

            var todayManagua = TimeZoneHelper.TodayInManagua();
            var (dayStartUtc, dayEndUtc) = TimeZoneHelper.GetManaguaDayRangeUtc(todayManagua);

            var totalOfferedGroups = await _repository.CountOfferedGroupsAsync(period.Id, cancellationToken);
            var sessionsToday = await _repository.GetDashboardTodaySessionsAsync(period.Id, dayStartUtc, dayEndUtc, cancellationToken);
            var upcomingEvents = await _repository.GetUpcomingEventsAsync(period.Id, dayStartUtc, 10, cancellationToken);

            return new DashboardSummaryResponseDto
            {
                ActiveSchoolPeriod = new DashboardSchoolPeriodDto
                {
                    Id = period.Id,
                    Name = period.Name
                },
                TotalOfferedGroupSubjects = totalOfferedGroups,
                TotalSessionsToday = sessionsToday.Count,
                TeacherAbsenceAlertsToday = sessionsToday.Count(s => s.AttendanceStatus is "AUSENTE" or "SUSTITUTO"),
                SessionsToday = sessionsToday
                    .Select(s => new DashboardTodaySessionDto
                    {
                        CalendarSessionId = s.CalendarSessionId,
                        SubjectName = s.SubjectName,
                        GroupCode = s.GroupCode,
                        ClassroomCode = s.ClassroomCode,
                        TeacherName = s.TeacherFullName,
                        StartDate = TimeZoneHelper.ToManagua(s.StartDate),
                        EndDate = TimeZoneHelper.ToManagua(s.EndDate),
                        AttendanceStatus = s.AttendanceStatus
                    })
                    .ToList(),
                UpcomingEvents = upcomingEvents
                    .Select(e => new DashboardUpcomingEventDto
                    {
                        HolidayId = e.HolidayId,
                        Name = e.Name,
                        EventType = e.EventTypeName,
                        StartDate = TimeZoneHelper.ToManagua(e.StartDate),
                        EndDate = TimeZoneHelper.ToManagua(e.EndDate)
                    })
                    .ToList()
            };
        }
    }
}