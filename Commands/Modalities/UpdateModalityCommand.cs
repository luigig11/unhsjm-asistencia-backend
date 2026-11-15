using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Modalities;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Modalities
{
    public sealed class UpdateModalityCommand : TransactionalCommandBase<ModalityDto>
    {
        private readonly IModalityRepository _modalityRepository;
        private Guid _id;
        private UpdateModalityDto? _request;

        public UpdateModalityCommand(IDapperRlsContext dbContext, IModalityRepository modalityRepository)
            : base(dbContext)
        {
            _modalityRepository = modalityRepository;
        }

        public UpdateModalityCommand WithRequest(Guid id, UpdateModalityDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("REqes tpayloar es requerido.");
            ListModalitiesCommand.ValidateFields(dto.Code, dto.Name);
        }

        protected override async Task<ModalityDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _modalityRepository.UpdateAsync(
                _id,
                new ModalityEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Modalidad con id '{_id}' no fue encontrado.");

            return ListModalitiesCommand.MapToDto(updated);
        }
    }

}
