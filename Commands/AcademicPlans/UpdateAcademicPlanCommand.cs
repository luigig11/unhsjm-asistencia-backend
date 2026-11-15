using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlans
{
    public sealed class UpdateAcademicPlanCommand : TransactionalCommandBase<AcademicPlanDto>
    {
        private readonly IAcademicPlanRepository _repository;
        private Guid _id;
        private UpdateAcademicPlanDto? _request;

        public UpdateAcademicPlanCommand(IDapperRlsContext dbContext, IAcademicPlanRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicPlanCommand WithRequest(Guid id, UpdateAcademicPlanDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicPlansCommand.ValidateFields(
                dto.UniversityCenterId,
                dto.CareerId,
                dto.AcademicPeriodTypeId,
                dto.ModalityId,
                dto.Code,
                dto.Name,
                dto.AcademicCycleCount,
                dto.AcademicPeriodCount,
                dto.ActiveFrom,
                dto.ActiveUntil);
        }

        protected override async Task<AcademicPlanDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicPlanEntity
                {
                    Id = _id,
                    UniversityCenterId = dto.UniversityCenterId,
                    CareerId = dto.CareerId,
                    AcademicPeriodTypeId = dto.AcademicPeriodTypeId,
                    ModalityId = dto.ModalityId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    AcademicCycleCount = dto.AcademicCycleCount,
                    AcademicPeriodCount = dto.AcademicPeriodCount,
                    ActiveFrom = dto.ActiveFrom,
                    ActiveUntil = dto.ActiveUntil,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Plan academico con id '{_id}' no fue encontrado.");

            return ListAcademicPlansCommand.MapToDto(updated);
        }
    }
}