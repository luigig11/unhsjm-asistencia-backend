using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicInstances;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicInstances
{
    public sealed class UpdateAcademicInstanceCommand : TransactionalCommandBase<AcademicInstanceDto>
    {
        private readonly IAcademicInstanceRepository _repository;
        private Guid _id;
        private UpdateAcademicInstanceDto? _request;

        public UpdateAcademicInstanceCommand(IDapperRlsContext dbContext, IAcademicInstanceRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicInstanceCommand WithRequest(Guid id, UpdateAcademicInstanceDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicInstanceEntity
                {
                    Id = _id,
                    SchoolPeriodId = dto.SchoolPeriodId,
                    AcademicPlanDetailId = dto.AcademicPlanDetailId,
                    Name = dto.Name.Trim(),
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Instancia academica con id '{_id}' no fue encontrada.");

            return ListAcademicInstancesCommand.MapToDto(updated);
        }
    }

}