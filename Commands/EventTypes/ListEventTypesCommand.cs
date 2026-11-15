using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.EventTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.EventTypes
{
    public sealed class ListEventTypesCommand : CommandBase<IReadOnlyList<EventTypeDto>>
    {
        private readonly IEventTypeRepository _repository;
        private bool? _isActive;

        public ListEventTypesCommand(IEventTypeRepository repository)
        {
            _repository = repository;
        }

        public ListEventTypesCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<EventTypeDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static EventTypeDto MapToDto(EventTypeEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .ValidateAndThrow();
        }
    }
}