using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriods
{
    public sealed class CreateAcademicPeriodCommand : TransactionalCommandBase<AcademicPeriodDto>
    {
        private readonly IAcademicPeriodRepository _repository;
        private CreateAcademicPeriodDto? _request;

        public CreateAcademicPeriodCommand(IDapperRlsContext dbContext, IAcademicPeriodRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicPeriodCommand WithRequest(CreateAcademicPeriodDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicPeriodsCommand.ValidateFields(dto.AcademicPeriodTypeId, dto.Code, dto.Name, dto.Order);
        }

        protected override async Task<AcademicPeriodDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new AcademicPeriodEntity
                {
                    AcademicPeriodTypeId = dto.AcademicPeriodTypeId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Order = dto.Order,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAcademicPeriodsCommand.MapToDto(created);
        }
    }
}
