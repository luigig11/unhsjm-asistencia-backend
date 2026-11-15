using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Modalities;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Modalities
{
    public sealed class CreateModalityCommand : TransactionalCommandBase<ModalityDto>
    {
        private readonly IModalityRepository _modalityRepository;
        private CreateModalityDto? _request;

        public CreateModalityCommand(IDapperRlsContext dbContext, IModalityRepository modalityRepository)
            : base(dbContext)
        {
            _modalityRepository = modalityRepository;
        }

        public CreateModalityCommand WithRequest(CreateModalityDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListModalitiesCommand.ValidateFields(dto.Code, dto.Name);
        }

        protected override async Task<ModalityDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _modalityRepository.CreateAsync(
                new ModalityEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListModalitiesCommand.MapToDto(created);
        }
    }
}
