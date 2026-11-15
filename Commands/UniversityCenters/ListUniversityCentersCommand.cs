using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.UniversityCenters;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.UniversityCenters
{
    public sealed class ListUniversityCentersCommand : CommandBase<IReadOnlyList<UniversityCenterDto>>
    {
        private readonly IUniversityCenterRepository _repository;
        private string? _search;
        private bool? _isActive;

        public ListUniversityCentersCommand(IUniversityCenterRepository repository)
        {
            _repository = repository;
        }

        public ListUniversityCentersCommand WithFilter(string? search, bool? isActive)
        {
            _search = search;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<UniversityCenterDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_search, _isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static UniversityCenterDto MapToDto(UniversityCenterEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Location = entity.Location,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name, string? location)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(150)
                .Field("ubicacion", location, "Location").MaxLength(200)
                .ValidateAndThrow();
        }
    }
}