using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CalendarSessions;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CalendarSessions
{
    public sealed class GenerateCalendarSessionsCommand : TransactionalCommandBase<GeneratedCalendarResponseDto>
    {
        private readonly IGroupSubjectRepository _groupSubjectRepository;
        private readonly IWeeklyScheduleRepository _weeklyScheduleRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ICalendarSessionRepository _calendarSessionRepository;
        private Guid _groupSubjectId;

        public GenerateCalendarSessionsCommand(
            IDapperRlsContext dbContext,
            IGroupSubjectRepository groupSubjectRepository,
            IWeeklyScheduleRepository weeklyScheduleRepository,
            IHolidayRepository holidayRepository,
            ICalendarSessionRepository calendarSessionRepository)
            : base(dbContext)
        {
            _groupSubjectRepository = groupSubjectRepository;
            _weeklyScheduleRepository = weeklyScheduleRepository;
            _holidayRepository = holidayRepository;
            _calendarSessionRepository = calendarSessionRepository;
        }

        public GenerateCalendarSessionsCommand WithGroupSubjectId(Guid groupSubjectId)
        {
            _groupSubjectId = groupSubjectId;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            Validator.Create()
                .Field("id_materia_por_grupo", _groupSubjectId, "Grupo de materia").RequiredGuid()
                .ValidateAndThrow();
        }

        protected override async Task<GeneratedCalendarResponseDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var context = await _groupSubjectRepository.GetGenerationContextAsync(_groupSubjectId, cancellationToken)
                ?? throw new NotFoundApiException($"GRupo de materia con id '{_groupSubjectId}' no fue encontrado");

            var weeklyBlocks = await _weeklyScheduleRepository.GetAllAsync(_groupSubjectId, isActive: true, cancellationToken);
            if (weeklyBlocks.Count == 0)
            {
                throw new ValidationApiException(
                    "Ocurrieron uno o mas errores de validacion.",
                    new Dictionary<string, string[]>
                    {
                        ["id_materia_por_grupo"] = ["No hay bloques de horario semanal activos configurados para este curso."]
                    });
            }

            var holidays = await _holidayRepository.GetAllAsync(context.SchoolPeriodId, eventTypeId: null, isActive: true, cancellationToken);

            var createdSessions = new List<GeneratedSessionItemDto>();
            var omittedHolidays = new List<OmittedHolidayDateDto>();
            var recordedOmissions = new HashSet<(DateOnly Date, string Reason)>();

            for (var currentDate = context.InstanceStartDate; currentDate <= context.InstanceEndDate; currentDate = currentDate.AddDays(1))
            {
                var dayId = currentDate.DayOfWeek == DayOfWeek.Sunday ? (short)7 : (short)currentDate.DayOfWeek;
                var blocksForDay = weeklyBlocks.Where(b => b.DayId == dayId).ToList();
                if (blocksForDay.Count == 0)
                {
                    continue;
                }

                foreach (var block in blocksForDay)
                {
                    var sessionStartUtc = TimeZoneHelper.CombineManaguaToUtc(currentDate, block.StartTime);
                    var sessionEndUtc = TimeZoneHelper.CombineManaguaToUtc(currentDate, block.EndTime);

                    var matchingHoliday = FindMatchingHoliday(holidays, currentDate, sessionStartUtc, sessionEndUtc);
                    if (matchingHoliday is not null)
                    {
                        if (recordedOmissions.Add((currentDate, matchingHoliday.Name)))
                        {
                            omittedHolidays.Add(new OmittedHolidayDateDto
                            {
                                Date = currentDate,
                                HolidayReason = matchingHoliday.Name
                            });
                        }

                        continue;
                    }

                    var inserted = await _calendarSessionRepository.TryInsertIfNotExistsAsync(
                        new CalendarSessionEntity
                        {
                            GroupSubjectId = _groupSubjectId,
                            ClassroomId = block.ClassroomId,
                            StartDate = sessionStartUtc,
                            EndDate = sessionEndUtc,
                            IsActive = true
                        },
                        cancellationToken);

                    if (inserted is not null)
                    {
                        createdSessions.Add(new GeneratedSessionItemDto
                        {
                            Id = inserted.Id,
                            GroupSubjectId = inserted.GroupSubjectId,
                            ClassroomId = inserted.ClassroomId,
                            ClassroomCode = block.ClassroomCode,
                            StartDate = TimeZoneHelper.ToManagua(inserted.StartDate),
                            EndDate = TimeZoneHelper.ToManagua(inserted.EndDate),
                            IsActive = inserted.IsActive
                        });
                    }
                }
            }

            return new GeneratedCalendarResponseDto
            {
                SessionsCreated = createdSessions.Count,
                OmittedHolidayDates = omittedHolidays,
                Sessions = createdSessions
            };
        }

        private static HolidayWithEventTypeEntity? FindMatchingHoliday(
            IReadOnlyList<HolidayWithEventTypeEntity> holidays,
            DateOnly sessionManaguaDate,
            DateTimeOffset sessionStartUtc,
            DateTimeOffset sessionEndUtc)
        {
            foreach (var holiday in holidays)
            {
                var holidayStartManagua = TimeZoneHelper.ToManagua(holiday.StartDate);
                var holidayEndManagua = TimeZoneHelper.ToManagua(holiday.EndDate);

                // Si los límites de los días festivos abarcan de medianoche a medianoche, considera ambas fechas como días completos, incluyéndolas.
                if (holidayStartManagua.TimeOfDay == TimeSpan.Zero && holidayEndManagua.TimeOfDay == TimeSpan.Zero)
                {
                    var holidayStartDate = DateOnly.FromDateTime(holidayStartManagua.DateTime);
                    var holidayEndDate = DateOnly.FromDateTime(holidayEndManagua.DateTime);
                    if (sessionManaguaDate >= holidayStartDate && sessionManaguaDate <= holidayEndDate)
                    {
                        return holiday;
                    }

                    continue;
                }

                var holidayStartUtc = TimeZoneHelper.ToUtc(holiday.StartDate);
                var holidayEndUtc = TimeZoneHelper.ToUtc(holiday.EndDate);
                if (holidayStartUtc <= sessionEndUtc && holidayEndUtc >= sessionStartUtc)
                {
                    return holiday;
                }
            }

            return null;
        }
    }
}