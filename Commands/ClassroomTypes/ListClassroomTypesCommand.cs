using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.ClassroomTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.ClassroomTypes
{
    public sealed class ListClassroomTypesCommand : CommandBase<IReadOnlyList<ClassroomTypeDto>>
    {
        private readonly IClassroomTypeRepository _repository;
        private bool? _isActive;

        public ListClassroomTypesCommand(IClassroomTypeRepository repository)
        {
            _repository = repository;
        }

        public ListClassroomTypesCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<ClassroomTypeDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static ClassroomTypeDto MapToDto(ClassroomTypeEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name, string? description)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("descripcion", description, "Description").MaxLength(250)
                .ValidateAndThrow();
        }
    }
}
