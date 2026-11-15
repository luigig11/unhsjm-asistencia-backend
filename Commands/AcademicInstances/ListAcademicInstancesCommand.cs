using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicInstances;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicInstances
{
    public sealed class ListAcademicInstancesCommand : CommandBase<IReadOnlyList<AcademicInstanceListItemDto>>
    {
        private readonly IAcademicInstanceRepository _repository;
        private Guid? _schoolPeriodId;
        private Guid? _academicPlanId;
        private bool? _isActive;

        public ListAcademicInstancesCommand(IAcademicInstanceRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicInstancesCommand WithFilter(Guid? schoolPeriodId, Guid? academicPlanId, bool? isActive)
        {
            _schoolPeriodId = schoolPeriodId;
            _academicPlanId = academicPlanId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicInstanceListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_schoolPeriodId, _academicPlanId, _isActive, cancellationToken);
            return items
                .Select(entity => new AcademicInstanceListItemDto
                {
                    Id = entity.Id,
                    SchoolPeriodId = entity.SchoolPeriodId,
                    SchoolPeriodName = entity.SchoolPeriodName,
                    AcademicPlanDetailId = entity.AcademicPlanDetailId,
                    AcademicPlanCode = entity.AcademicPlanCode,
                    AcademicCycleName = entity.AcademicCycleName,
                    AcademicPeriodName = entity.AcademicPeriodName,
                    Name = entity.Name,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static AcademicInstanceDto MapToDto(AcademicInstanceEntity entity) => new()
        {
            Id = entity.Id,
            SchoolPeriodId = entity.SchoolPeriodId,
            AcademicPlanDetailId = entity.AcademicPlanDetailId,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid schoolPeriodId,
            Guid academicPlanDetailId,
            string? name,
            DateOnly startDate,
            DateOnly endDate)
        {
            Validator.Create()
                .Field("id_periodo_escolar", schoolPeriodId, "Periodo escolar").RequiredGuid()
                .Field("id_detalle_plan_academico", academicPlanDetailId, "Detalle de plan academico").RequiredGuid()
                .Field("nombre", name, "Nombre").NotEmpty().MaxLength(50)
                .Rule("fecha_fin", () => endDate > startDate, "Fecha final debe ser mayor que fecha de inicio.")
                .ValidateAndThrow();
        }
    }
}