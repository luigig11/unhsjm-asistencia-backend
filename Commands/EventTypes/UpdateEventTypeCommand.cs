using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.EventTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.EventTypes
{
    public sealed class UpdateEventTypeCommand : TransactionalCommandBase<EventTypeDto>
    {
        private readonly IEventTypeRepository _repository;
        private Guid _id;
        private UpdateEventTypeDto? _request;

        public UpdateEventTypeCommand(IDapperRlsContext dbContext, IEventTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateEventTypeCommand WithRequest(Guid id, UpdateEventTypeDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new EventTypeEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Tipo de evento con id '{_id}' no fue encontrado.");

            return ListEventTypesCommand.MapToDto(updated);
        }
    }
}