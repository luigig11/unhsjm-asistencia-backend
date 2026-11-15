using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPeriodTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriodTypes
{
    public sealed class ListAcademicPeriodTypesCommand : CommandBase<IReadOnlyList<AcademicPeriodTypeDto>>
    {
        private readonly IAcademicPeriodTypeRepository _repository;
        private bool? _isActive;

        public ListAcademicPeriodTypesCommand(IAcademicPeriodTypeRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicPeriodTypesCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicPeriodTypeDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static AcademicPeriodTypeDto MapToDto(AcademicPeriodTypeEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            MinMonthsDuration = entity.MinMonthsDuration,
            MaxMonthsDuration = entity.MaxMonthsDuration,
            MinHoursDuration = entity.MinHoursDuration,
            MaxHoursDuration = entity.MaxHoursDuration,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            string? code,
            string? name,
            short minMonths,
            short maxMonths,
            short minHours,
            short maxHours)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("duracion_meses_minimo", minMonths, "Duracion minima en meses").Positive()
                .Rule("duracion_meses_maximo", () => maxMonths >= minMonths, "Duracion maxima debe ser mayor o igual que la duracion minima.")
                .Field("duracion_horas_minimo", minHours, "Duracion minima en horas").Positive()
                .Rule("duracion_horas_maximo", () => maxHours >= minHours, "Duracion maxima en horas debe ser mayor o igual que la minima.")
                .ValidateAndThrow();
        }
    }
}
