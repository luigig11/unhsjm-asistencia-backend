using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AbsenceReasons;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AbsenceReasons
{
    public sealed class ListAbsenceReasonsCommand : CommandBase<IReadOnlyList<AbsenceReasonDto>>
    {
        private static readonly HashSet<string> ValidTargets = new(StringComparer.OrdinalIgnoreCase)
        {
            "DOCENTE", "ESTUDIANTE", "AMBOS"
        };

        private readonly IAbsenceReasonRepository _repository;
        private string? _appliesTo;
        private bool? _isActive;

        public ListAbsenceReasonsCommand(IAbsenceReasonRepository repository)
        {
            _repository = repository;
        }

        public ListAbsenceReasonsCommand WithFilter(string? appliesTo, bool? isActive)
        {
            _appliesTo = appliesTo;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AbsenceReasonDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_appliesTo, _isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static AbsenceReasonDto MapToDto(AbsenceReasonEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            JustifiesAbsence = entity.JustifiesAbsence,
            AppliesTo = entity.AppliesTo,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name, string? description, string? appliesTo)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("descripcion", description, "Description").MaxLength(250)
                .Field("aplica_a", appliesTo, "aplica_a").AllowedValues(ValidTargets, "aplica_a debe ser 'DOCENTE', 'ESTUDIANTE', o 'AMBOS'.")
                .ValidateAndThrow();
        }
    }
}
