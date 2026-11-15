using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.SchoolPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.SchoolPeriods
{
    public sealed class ListSchoolPeriodsCommand : CommandBase<IReadOnlyList<SchoolPeriodDto>>
    {
        private readonly ISchoolPeriodRepository _repository;
        private bool? _isActive;

        public ListSchoolPeriodsCommand(ISchoolPeriodRepository repository)
        {
            _repository = repository;
        }

        public ListSchoolPeriodsCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<SchoolPeriodDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static SchoolPeriodDto MapToDto(SchoolPeriodEntity entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? name, DateOnly startDate, DateOnly endDate)
        {
            Validator.Create()
                .Field("nombre_periodo_escolar", name, "nombre de periodo escolar").NotEmpty().MaxLength(50)
                .Rule("fecha_fin", () => endDate > startDate, "fecha de fin debe ser mayor que fecha de inicio.")
                .ValidateAndThrow();
        }
    }
}