using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPlanDetails;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlanDetails
{
    public sealed class UpdateAcademicPlanDetailCommand : TransactionalCommandBase<AcademicPlanDetailDto>
    {
        private readonly IAcademicPlanDetailRepository _repository;
        private Guid _id;
        private UpdateAcademicPlanDetailDto? _request;

        public UpdateAcademicPlanDetailCommand(IDapperRlsContext dbContext, IAcademicPlanDetailRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicPlanDetailCommand WithRequest(Guid id, UpdateAcademicPlanDetailDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido");
            ListAcademicPlanDetailsCommand.ValidateFields(
                _id,
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
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicPlanDetailEntity
                {
                    Id = _id,
                    AcademicPlanId = dto.AcademicPlanId,
                    AcademicCycleId = dto.AcademicCycleId,
                    AcademicPeriodId = dto.AcademicPeriodId,
                    DurationWeeks = dto.DurationWeeks,
                    DurationMonths = dto.DurationMonths,
                    PredecessorDetailId = dto.PredecessorDetailId,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Detalle de plan academico con id '{_id}' no fue encontrado.");

            return ListAcademicPlanDetailsCommand.MapToDto(updated);
        }
    }
}