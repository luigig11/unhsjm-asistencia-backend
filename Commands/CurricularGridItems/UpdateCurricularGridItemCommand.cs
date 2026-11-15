using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CurricularGridItems;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CurricularGridItems
{
    public sealed class UpdateCurricularGridItemCommand : TransactionalCommandBase<CurricularGridItemDto>
    {
        private readonly ICurricularGridRepository _gridRepository;
        private readonly ISubjectPrerequisiteRepository _prerequisiteRepository;
        private Guid _id;
        private UpdateCurricularGridItemDto? _request;

        public UpdateCurricularGridItemCommand(
            IDapperRlsContext dbContext,
            ICurricularGridRepository gridRepository,
            ISubjectPrerequisiteRepository prerequisiteRepository)
            : base(dbContext)
        {
            _gridRepository = gridRepository;
            _prerequisiteRepository = prerequisiteRepository;
        }

        public UpdateCurricularGridItemCommand WithRequest(Guid id, UpdateCurricularGridItemDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            CurricularGridItemHelper.ValidateFields(
                _id,
                dto.AcademicPlanDetailId,
                dto.SubjectId,
                dto.TotalMeetings,
                dto.MeetingsPerWeek,
                dto.SubjectHours,
                dto.Credits,
                dto.PrerequisiteCurricularGridItemIds);
        }

        protected override async Task<CurricularGridItemDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _gridRepository.UpdateAsync(
                _id,
                new CurricularGridItemEntity
                {
                    Id = _id,
                    AcademicPlanDetailId = dto.AcademicPlanDetailId,
                    SubjectId = dto.SubjectId,
                    TotalMeetings = dto.TotalMeetings,
                    MeetingsPerWeek = dto.MeetingsPerWeek,
                    SubjectHours = dto.SubjectHours,
                    Credits = dto.Credits,
                    IsMandatory = dto.IsMandatory,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Item de malla curricular con id '{_id}' no fue encontrado.");

            IReadOnlyList<Guid> prerequisiteIds;
            if (dto.PrerequisiteCurricularGridItemIds is not null)
            {
                prerequisiteIds = dto.PrerequisiteCurricularGridItemIds;
                await _prerequisiteRepository.ReplacePrerequisitesAsync(_id, prerequisiteIds, cancellationToken);
            }
            else
            {
                prerequisiteIds = await _prerequisiteRepository.GetPrerequisiteGridItemIdsAsync(_id, cancellationToken);
            }

            return CurricularGridItemHelper.MapToDto(updated, prerequisiteIds);
        }
    }
}