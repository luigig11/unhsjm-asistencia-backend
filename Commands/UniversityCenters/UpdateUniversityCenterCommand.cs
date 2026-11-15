using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.UniversityCenters;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.UniversityCenters
{
    public sealed class UpdateUniversityCenterCommand : TransactionalCommandBase<UniversityCenterDto>
    {
        private readonly IUniversityCenterRepository _repository;
        private Guid _id;
        private UpdateUniversityCenterDto? _request;

        public UpdateUniversityCenterCommand(IDapperRlsContext dbContext, IUniversityCenterRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateUniversityCenterCommand WithRequest(Guid id, UpdateUniversityCenterDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new UniversityCenterEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Centro universitario con id '{_id}' no fue encontrado.");

            return ListUniversityCentersCommand.MapToDto(updated);
        }
    }
}