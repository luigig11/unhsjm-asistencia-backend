using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Holidays;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Holidays
{
    public sealed class CreateHolidayCommand : TransactionalCommandBase<HolidayDto>
    {
        private readonly IHolidayRepository _repository;
        private CreateHolidayDto? _request;

        public CreateHolidayCommand(IDapperRlsContext dbContext, IHolidayRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateHolidayCommand WithRequest(CreateHolidayDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListHolidaysCommand.ValidateFields(dto.SchoolPeriodId, dto.EventTypeId, dto.Name, dto.StartDate, dto.EndDate);
        }

        protected override async Task<HolidayDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new HolidayEntity
                {
                    SchoolPeriodId = dto.SchoolPeriodId,
                    EventTypeId = dto.EventTypeId,
                    Name = dto.Name.Trim(),
                    StartDate = TimeZoneHelper.ToUtc(dto.StartDate),
                    EndDate = TimeZoneHelper.ToUtc(dto.EndDate),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListHolidaysCommand.MapToDto(created);
        }
    }
}