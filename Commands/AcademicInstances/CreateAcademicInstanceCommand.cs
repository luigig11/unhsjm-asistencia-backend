using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicInstances;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicInstances
{
    public sealed class CreateAcademicInstanceCommand : TransactionalCommandBase<AcademicInstanceDto>
    {
        private readonly IAcademicInstanceRepository _repository;
        private CreateAcademicInstanceDto? _request;

        public CreateAcademicInstanceCommand(IDapperRlsContext dbContext, IAcademicInstanceRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicInstanceCommand WithRequest(CreateAcademicInstanceDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicInstancesCommand.ValidateFields(
                dto.SchoolPeriodId,
                dto.AcademicPlanDetailId,
                dto.Name,
                dto.StartDate,
                dto.EndDate);
        }

        protected override async Task<AcademicInstanceDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new AcademicInstanceEntity
                {
                    SchoolPeriodId = dto.SchoolPeriodId,
                    AcademicPlanDetailId = dto.AcademicPlanDetailId,
                    Name = dto.Name.Trim(),
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAcademicInstancesCommand.MapToDto(created);
        }
    }

}