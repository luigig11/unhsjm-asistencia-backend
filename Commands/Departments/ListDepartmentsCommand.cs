using System.ComponentModel.DataAnnotations;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Departments;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Departments
{
    public sealed class ListDepartmentsCommand : CommandBase<IReadOnlyList<DepartmentDto>>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private string? _search;
        private bool? _isActive;

        public ListDepartmentsCommand(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public ListDepartmentsCommand WithFilter(string? search, bool? isActive)
        {
            _search = search;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<DepartmentDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _departmentRepository.GetAllAsync(_search, _isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static DepartmentDto MapToDto(DepartmentEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name)
        {
            Abstractions.Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(150)
                .ValidateAndThrow();
        }
    }
}
