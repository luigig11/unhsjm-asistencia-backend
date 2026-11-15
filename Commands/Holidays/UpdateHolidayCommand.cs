using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Holidays;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Holidays
{
    public sealed class UpdateHolidayCommand : TransactionalCommandBase<HolidayDto>
    {
        private readonly IHolidayRepository _repository;
        private Guid _id;
        private UpdateHolidayDto? _request;

        public UpdateHolidayCommand(IDapperRlsContext dbContext, IHolidayRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateHolidayCommand WithRequest(Guid id, UpdateHolidayDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new HolidayEntity
                {
                    Id = _id,
                    SchoolPeriodId = dto.SchoolPeriodId,
                    EventTypeId = dto.EventTypeId,
                    Name = dto.Name.Trim(),
                    StartDate = TimeZoneHelper.ToUtc(dto.StartDate),
                    EndDate = TimeZoneHelper.ToUtc(dto.EndDate),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Dia feriado con id '{_id}' no fue encontrado.");

            return ListHolidaysCommand.MapToDto(updated);
        }
    }
}