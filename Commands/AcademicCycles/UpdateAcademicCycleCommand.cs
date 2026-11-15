using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicCycles;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicCycles
{
    public sealed class UpdateAcademicCycleCommand : TransactionalCommandBase<AcademicCycleDto>
    {
        private readonly IAcademicCycleRepository _repository;
        private Guid _id;
        private UpdateAcademicCycleDto? _request;

        public UpdateAcademicCycleCommand(IDapperRlsContext dbContext, IAcademicCycleRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicCycleCommand WithRequest(Guid id, UpdateAcademicCycleDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicCyclesCommand.ValidateFields(dto.Code, dto.Name, dto.Order);
        }

        protected override async Task<AcademicCycleDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicCycleEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Order = dto.Order,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Ciclo academico con id '{_id}' no fue encontrado.");

            return ListAcademicCyclesCommand.MapToDto(updated);
        }
    }
}
