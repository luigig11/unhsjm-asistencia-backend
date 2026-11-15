using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicCycles;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicCycles
{
    public sealed class ListAcademicCyclesCommand : CommandBase<IReadOnlyList<AcademicCycleDto>>
    {
        private readonly IAcademicCycleRepository _repository;
        private bool? _isActive;

        public ListAcademicCyclesCommand(IAcademicCycleRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicCyclesCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicCycleDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static AcademicCycleDto MapToDto(AcademicCycleEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Order = entity.Order,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name, short order)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("orden", order, "Order").Between(1, 10, "Order must be between 1 and 10.")
                .ValidateAndThrow();
        }
    }
}
