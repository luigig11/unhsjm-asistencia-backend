using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Classrooms;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Classrooms
{
    public sealed class ListClassroomsCommand : CommandBase<IReadOnlyList<ClassroomListItemDto>>
    {
        private readonly IClassroomRepository _repository;
        private Guid? _classroomTypeId;
        private string? _search;
        private bool? _isActive;

        public ListClassroomsCommand(IClassroomRepository repository)
        {
            _repository = repository;
        }

        public ListClassroomsCommand WithFilter(Guid? classroomTypeId, string? search, bool? isActive)
        {
            _classroomTypeId = classroomTypeId;
            _search = search;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<ClassroomListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_classroomTypeId, _search, _isActive, cancellationToken);
            return items
                .Select(c => new ClassroomListItemDto
                {
                    Id = c.Id,
                    ClassroomTypeId = c.ClassroomTypeId,
                    ClassroomTypeName = c.ClassroomTypeName,
                    Code = c.Code,
                    Name = c.Name,
                    Capacity = c.Capacity,
                    Location = c.Location,
                    IsActive = c.IsActive
                })
                .ToList();
        }

        internal static ClassroomDto MapToDto(ClassroomEntity entity) => new()
        {
            Id = entity.Id,
            ClassroomTypeId = entity.ClassroomTypeId,
            Code = entity.Code,
            Name = entity.Name,
            Capacity = entity.Capacity,
            Location = entity.Location,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid classroomTypeId,
            string? code,
            string? name,
            short capacity,
            string? location)
        {
            Validator.Create()
                .Field("id_tipo_aula", classroomTypeId, "classroom type").RequiredGuid()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("ubicacion", location, "Location").MaxLength(150)
                .Field("capacidad", capacity, "Capacity").Positive()
                .ValidateAndThrow();
        }
    }
}
