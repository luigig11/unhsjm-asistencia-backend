using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlans
{
    public sealed class ListAcademicPlansCommand : CommandBase<IReadOnlyList<AcademicPlanListItemDto>>
    {
        private readonly IAcademicPlanRepository _repository;
        private Guid? _departmentId;
        private Guid? _careerId;
        private Guid? _modalityId;
        private Guid? _universityCenterId;
        private bool? _isActive;

        public ListAcademicPlansCommand(IAcademicPlanRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicPlansCommand WithFilter(
            Guid? departmentId,
            Guid? careerId,
            Guid? modalityId,
            Guid? universityCenterId,
            bool? isActive)
        {
            _departmentId = departmentId;
            _careerId = careerId;
            _modalityId = modalityId;
            _universityCenterId = universityCenterId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicPlanListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_departmentId, _careerId, _modalityId, _universityCenterId, _isActive, cancellationToken);
            return items.Select(MapToListItemDto).ToList();
        }

        internal static AcademicPlanListItemDto MapToListItemDto(AcademicPlanWithDetailsEntity entity) => new()
        {
            Id = entity.Id,
            UniversityCenterId = entity.UniversityCenterId,
            UniversityCenterName = entity.UniversityCenterName,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.DepartmentName,
            CareerId = entity.CareerId,
            CareerName = entity.CareerName,
            AcademicPeriodTypeId = entity.AcademicPeriodTypeId,
            AcademicPeriodTypeName = entity.AcademicPeriodTypeName,
            ModalityId = entity.ModalityId,
            ModalityName = entity.ModalityName,
            Code = entity.Code,
            Name = entity.Name,
            AcademicCycleCount = entity.AcademicCycleCount,
            AcademicPeriodCount = entity.AcademicPeriodCount,
            ActiveFrom = entity.ActiveFrom,
            ActiveUntil = entity.ActiveUntil,
            IsActive = entity.IsActive
        };

        internal static AcademicPlanDto MapToDto(AcademicPlanEntity entity) => new()
        {
            Id = entity.Id,
            UniversityCenterId = entity.UniversityCenterId,
            CareerId = entity.CareerId,
            AcademicPeriodTypeId = entity.AcademicPeriodTypeId,
            ModalityId = entity.ModalityId,
            Code = entity.Code,
            Name = entity.Name,
            AcademicCycleCount = entity.AcademicCycleCount,
            AcademicPeriodCount = entity.AcademicPeriodCount,
            ActiveFrom = entity.ActiveFrom,
            ActiveUntil = entity.ActiveUntil,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(
            Guid universityCenterId,
            Guid careerId,
            Guid academicPeriodTypeId,
            Guid modalityId,
            string? code,
            string? name,
            short academicCycleCount,
            short academicPeriodCount,
            DateOnly activeFrom,
            DateOnly? activeUntil)
        {
            Validator.Create()
                .Field("id_centro_universitario", universityCenterId, "Centro universitario").RequiredGuid()
                .Field("id_carrera", careerId, "career").RequiredGuid()
                .Field("id_tipo_periodo_academico", academicPeriodTypeId, "tipo de periodo academico").RequiredGuid()
                .Field("id_modalidad", modalityId, "modality").RequiredGuid()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(30)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(150)
                .Field("cantidad_ciclo_academico", academicCycleCount, "Cantidad de ciclo academico").Positive()
                .Field("cantidad_periodo_academico", academicPeriodCount, "Canidad de periodo academico").Positive()
                .Rule("activo_hasta", () => !activeUntil.HasValue || activeUntil.Value > activeFrom, "El campo activo hasta debe ser mayor que la fecha activo desde.")
                .ValidateAndThrow();
        }
    }

}