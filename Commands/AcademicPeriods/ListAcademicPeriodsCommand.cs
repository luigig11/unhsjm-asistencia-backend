using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPeriods;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriods
{
    public sealed class ListAcademicPeriodsCommand : CommandBase<IReadOnlyList<AcademicPeriodListItemDto>>
    {
        private readonly IAcademicPeriodRepository _repository;
        private Guid? _academicPeriodTypeId;
        private bool? _isActive;

        public ListAcademicPeriodsCommand(IAcademicPeriodRepository repository)
        {
            _repository = repository;
        }

        public ListAcademicPeriodsCommand WithFilter(Guid? academicPeriodTypeId, bool? isActive)
        {
            _academicPeriodTypeId = academicPeriodTypeId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<AcademicPeriodListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_academicPeriodTypeId, _isActive, cancellationToken);
            return items
                .Select(p => new AcademicPeriodListItemDto
                {
                    Id = p.Id,
                    AcademicPeriodTypeId = p.AcademicPeriodTypeId,
                    AcademicPeriodTypeName = p.AcademicPeriodTypeName,
                    Code = p.Code,
                    Name = p.Name,
                    Order = p.Order,
                    IsActive = p.IsActive
                })
                .ToList();
        }

        internal static AcademicPeriodDto MapToDto(AcademicPeriodEntity entity) => new()
        {
            Id = entity.Id,
            AcademicPeriodTypeId = entity.AcademicPeriodTypeId,
            Code = entity.Code,
            Name = entity.Name,
            Order = entity.Order,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(Guid periodTypeId, string? code, string? name, short order)
        {
            Validator.Create()
                .Field("id_tipo_periodo_academico", periodTypeId, "academic period type").RequiredGuid()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(20)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(100)
                .Field("orden", order, "Order").Positive()
                .ValidateAndThrow();
        }
    }
}
