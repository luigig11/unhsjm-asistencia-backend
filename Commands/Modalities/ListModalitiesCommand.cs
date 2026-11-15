using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Modalities;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Modalities
{
    public sealed class ListModalitiesCommand : CommandBase<IReadOnlyList<ModalityDto>>
    {
        private readonly IModalityRepository _modalityRepository;
        private string? _search;
        private bool? _isActive;

        public ListModalitiesCommand(IModalityRepository modalityRepository)
        {
            _modalityRepository = modalityRepository;
        }

        public ListModalitiesCommand WithFilter(string? search, bool? isActive)
        {
            _search = search;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<ModalityDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _modalityRepository.GetAllAsync(_search, _isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static ModalityDto MapToDto(ModalityEntity entity) => new()
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
