using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Carrers;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Carrers
{
    public sealed class ListCareersCommand : CommandBase<IReadOnlyList<CareerListItemDto>>
    {
        private readonly ICareerRepository _careerRepository;
        private string? _search;
        private Guid? _departmentId;
        private bool? _isActive;

        public ListCareersCommand(ICareerRepository careerRepository)
        {
            _careerRepository = careerRepository;
        }

        public ListCareersCommand WithFilter(string? search, Guid? departmentId, bool? isActive)
        {
            _search = search;
            _departmentId = departmentId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<CareerListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _careerRepository.GetAllAsync(_search, _departmentId, _isActive, cancellationToken);
            return items
                .Select(c => new CareerListItemDto
                {
                    Id = c.Id,
                    DepartmentId = c.DepartmentId,
                    DepartmentName = c.DepartmentName,
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive
                })
                .ToList();
        }

        internal static CareerDto MapToDto(CareerEntity entity) => new()
        {
            Id = entity.Id,
            DepartmentId = entity.DepartmentId,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(Guid departmentId, string? code, string? name)
        {
            Abstractions.Validator.Create()
                .Field("id_direccion", departmentId, "department").RequiredGuid()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(150)
                .ValidateAndThrow();
        }
    }
}
