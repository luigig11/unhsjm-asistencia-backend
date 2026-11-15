using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.UniversityCenters;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.UniversityCenters
{
    public sealed class CreateUniversityCenterCommand : TransactionalCommandBase<UniversityCenterDto>
    {
        private readonly IUniversityCenterRepository _repository;
        private CreateUniversityCenterDto? _request;

        public CreateUniversityCenterCommand(IDapperRlsContext dbContext, IUniversityCenterRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateUniversityCenterCommand WithRequest(CreateUniversityCenterDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListUniversityCentersCommand.ValidateFields(dto.Code, dto.Name, dto.Location);
        }

        protected override async Task<UniversityCenterDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new UniversityCenterEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListUniversityCentersCommand.MapToDto(created);
        }
    }
}