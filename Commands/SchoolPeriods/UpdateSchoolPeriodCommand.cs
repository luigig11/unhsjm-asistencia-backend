using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.SchoolPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.SchoolPeriods
{
    public sealed class UpdateSchoolPeriodCommand : TransactionalCommandBase<SchoolPeriodDto>
    {
        private readonly ISchoolPeriodRepository _repository;
        private Guid _id;
        private UpdateSchoolPeriodDto? _request;

        public UpdateSchoolPeriodCommand(IDapperRlsContext dbContext, ISchoolPeriodRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateSchoolPeriodCommand WithRequest(Guid id, UpdateSchoolPeriodDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListSchoolPeriodsCommand.ValidateFields(dto.Name, dto.StartDate, dto.EndDate);
        }

        protected override async Task<SchoolPeriodDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new SchoolPeriodEntity
                {
                    Id = _id,
                    Name = dto.Name.Trim(),
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Periodo escolar con id '{_id}' no fue encontrado.");

            return ListSchoolPeriodsCommand.MapToDto(updated);
        }
    }
}