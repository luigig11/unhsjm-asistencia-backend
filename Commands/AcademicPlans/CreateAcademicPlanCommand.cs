using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlans
{
    public sealed class CreateAcademicPlanCommand : TransactionalCommandBase<AcademicPlanDto>
    {
        private readonly IAcademicPlanRepository _repository;
        private CreateAcademicPlanDto? _request;

        public CreateAcademicPlanCommand(IDapperRlsContext dbContext, IAcademicPlanRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicPlanCommand WithRequest(CreateAcademicPlanDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload no es requerido.");
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
            var created = await _repository.CreateAsync(
                new AcademicPlanEntity
                {
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
                cancellationToken);

            return ListAcademicPlansCommand.MapToDto(created);
        }
    }
}