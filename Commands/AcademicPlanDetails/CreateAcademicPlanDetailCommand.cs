using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPlanDetails;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlanDetails
{
    public sealed class CreateAcademicPlanDetailCommand : TransactionalCommandBase<AcademicPlanDetailDto>
    {
        private readonly IAcademicPlanDetailRepository _repository;
        private CreateAcademicPlanDetailDto? _request;

        public CreateAcademicPlanDetailCommand(IDapperRlsContext dbContext, IAcademicPlanDetailRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicPlanDetailCommand WithRequest(CreateAcademicPlanDetailDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicPlanDetailsCommand.ValidateFields(
                null,
                dto.AcademicPlanId,
                dto.AcademicCycleId,
                dto.AcademicPeriodId,
                dto.DurationWeeks,
                dto.DurationMonths,
                dto.PredecessorDetailId);
        }

        protected override async Task<AcademicPlanDetailDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new AcademicPlanDetailEntity
                {
                    AcademicPlanId = dto.AcademicPlanId,
                    AcademicCycleId = dto.AcademicCycleId,
                    AcademicPeriodId = dto.AcademicPeriodId,
                    DurationWeeks = dto.DurationWeeks,
                    DurationMonths = dto.DurationMonths,
                    PredecessorDetailId = dto.PredecessorDetailId,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAcademicPlanDetailsCommand.MapToDto(created);
        }
    }
}