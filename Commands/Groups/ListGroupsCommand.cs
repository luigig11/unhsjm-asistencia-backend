using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Groups;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Groups
{
    public sealed class ListGroupsCommand : CommandBase<IReadOnlyList<GroupListItemDto>>
    {
        private readonly IGroupRepository _repository;
        private Guid? _academicInstanceId;
        private bool? _isActive;

        public ListGroupsCommand(IGroupRepository repository)
        {
            _repository = repository;
        }

        public ListGroupsCommand WithFilter(Guid? academicInstanceId, bool? isActive)
        {
            _academicInstanceId = academicInstanceId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<GroupListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_academicInstanceId, _isActive, cancellationToken);
            return items
                .Select(entity => new GroupListItemDto
                {
                    Id = entity.Id,
                    AcademicInstanceId = entity.AcademicInstanceId,
                    InstanceName = entity.InstanceName,
                    Code = entity.Code,
                    Name = entity.Name,
                    Capacity = entity.Capacity,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static GroupDto MapToDto(GroupEntity entity) => new()
        {
            Id = entity.Id,
            AcademicInstanceId = entity.AcademicInstanceId,
            Code = entity.Code,
            Name = entity.Name,
            Capacity = entity.Capacity,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(Guid academicInstanceId, string? code, string? name, short capacity)
        {
            Validator.Create()
                .Field("id_instancia_academica", academicInstanceId, "instancia academica").RequiredGuid()
                .Field("codigo", code, "codigo").NotEmpty().MaxLength(20)
                .Field("nombre", name, "nombre").NotEmpty().MaxLength(100)
                .Field("cupo", capacity, "capacidad").Positive()
                .ValidateAndThrow();
        }
    }
}