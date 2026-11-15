using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPlanDetails;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlanDetails
{
    public sealed class ListAcademicPlanDetailsCommand : CommandBase<IReadOnlyList<AcademicPlanDetailListItemDto>>
    {
        private readonly IAcademicPlanDetailRepository _repository;
        private Guid? _academicPlanId;
        private bool? _isActive;

        public ListAcademicPlanDetailsCommand(IAcademicPlanDetailRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicPlanDetailsCommand WithFilter(Guid? academicPlanId, bool? isActive)
        {
            _academicPlanId = academicPlanId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicPlanDetailListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_academicPlanId, _isActive, cancellationToken);
            return items
                .Select(entity => new AcademicPlanDetailListItemDto
                {
                    Id = entity.Id,
                    AcademicPlanId = entity.AcademicPlanId,
                    AcademicCycleId = entity.AcademicCycleId,
                    AcademicCycleCode = entity.AcademicCycleCode,
                    AcademicCycleName = entity.AcademicCycleName,
                    AcademicCycleOrder = entity.AcademicCycleOrder,
                    AcademicPeriodId = entity.AcademicPeriodId,
                    AcademicPeriodCode = entity.AcademicPeriodCode,
                    AcademicPeriodName = entity.AcademicPeriodName,
                    AcademicPeriodOrder = entity.AcademicPeriodOrder,
                    DurationWeeks = entity.DurationWeeks,
                    DurationMonths = entity.DurationMonths,
                    PredecessorDetailId = entity.PredecessorDetailId,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static AcademicPlanDetailDto MapToDto(AcademicPlanDetailEntity entity) => new()
        {
            Id = entity.Id,
            AcademicPlanId = entity.AcademicPlanId,
            AcademicCycleId = entity.AcademicCycleId,
            AcademicPeriodId = entity.AcademicPeriodId,
            DurationWeeks = entity.DurationWeeks,
            DurationMonths = entity.DurationMonths,
            PredecessorDetailId = entity.PredecessorDetailId,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid? detailId,
            Guid academicPlanId,
            Guid academicCycleId,
            Guid academicPeriodId,
            short durationWeeks,
            short durationMonths,
            Guid? predecessorDetailId)
        {
            var validator = Validator.Create()
                .Field("id_plan_academico", academicPlanId, "academic plan").RequiredGuid()
                .Field("id_ciclo_academico", academicCycleId, "academic cycle").RequiredGuid()
                .Field("id_periodo_academico", academicPeriodId, "academic period").RequiredGuid()
                .Field("duracion_semanas", durationWeeks, "Duration in weeks").Between(1, 52)
                .Field("duracion_meses", durationMonths, "Duration in months").Between(1, 12);

            if (predecessorDetailId.HasValue)
            {
                validator.Field("id_detalle_predecesor", predecessorDetailId.Value, "predecessor detail")
                    .Must(() => predecessorDetailId.Value != Guid.Empty, "Predecessor detail UUID cannot be empty.");

                if (detailId.HasValue)
                {
                    validator.Must(
                        () => predecessorDetailId.Value != detailId.Value,
                        "An academic plan detail cannot reference itself as predecessor.");
                }
            }

            validator.ValidateAndThrow();
        }
    }
}