using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.EventTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.EventTypes
{
    public sealed class CreateEventTypeCommand : TransactionalCommandBase<EventTypeDto>
    {
        private readonly IEventTypeRepository _repository;
        private CreateEventTypeDto? _request;

        public CreateEventTypeCommand(IDapperRlsContext dbContext, IEventTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateEventTypeCommand WithRequest(CreateEventTypeDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListEventTypesCommand.ValidateFields(dto.Code, dto.Name);
        }

        protected override async Task<EventTypeDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new EventTypeEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListEventTypesCommand.MapToDto(created);
        }
    }
}



