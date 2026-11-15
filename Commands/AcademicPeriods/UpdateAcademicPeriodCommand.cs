using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriods
{
    public sealed class UpdateAcademicPeriodCommand : TransactionalCommandBase<AcademicPeriodDto>
    {
        private readonly IAcademicPeriodRepository _repository;
        private Guid _id;
        private UpdateAcademicPeriodDto? _request;

        public UpdateAcademicPeriodCommand(IDapperRlsContext dbContext, IAcademicPeriodRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicPeriodCommand WithRequest(Guid id, UpdateAcademicPeriodDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicPeriodEntity
                {
                    Id = _id,
                    AcademicPeriodTypeId = dto.AcademicPeriodTypeId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Order = dto.Order,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Periodo academico con id '{_id}' no fue encontrado.");

            return ListAcademicPeriodsCommand.MapToDto(updated);
        }
    }
}
