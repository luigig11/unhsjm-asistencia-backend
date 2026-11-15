using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.SchoolPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.SchoolPeriods
{
    public sealed class CreateSchoolPeriodCommand : TransactionalCommandBase<SchoolPeriodDto>
    {
        private readonly ISchoolPeriodRepository _repository;
        private CreateSchoolPeriodDto? _request;

        public CreateSchoolPeriodCommand(IDapperRlsContext dbContext, ISchoolPeriodRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateSchoolPeriodCommand WithRequest(CreateSchoolPeriodDto request)
        {
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
            var created = await _repository.CreateAsync(
                new SchoolPeriodEntity
                {
                    Name = dto.Name.Trim(),
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListSchoolPeriodsCommand.MapToDto(created);
        }
    }
}