using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicCycles;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicCycles
{
    public sealed class CreateAcademicCycleCommand : TransactionalCommandBase<AcademicCycleDto>
    {
        private readonly IAcademicCycleRepository _repository;
        private CreateAcademicCycleDto? _request;

        public CreateAcademicCycleCommand(IDapperRlsContext dbContext, IAcademicCycleRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicCycleCommand WithRequest(CreateAcademicCycleDto request)
        {
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
            var created = await _repository.CreateAsync(
                new AcademicCycleEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Order = dto.Order,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAcademicCyclesCommand.MapToDto(created);
        }
    }
}
