using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.CurricularGridItems;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.CurricularGridItems
{
    public sealed class CreateCurricularGridItemCommand : TransactionalCommandBase<CurricularGridItemDto>
    {
        private readonly ICurricularGridRepository _gridRepository;
        private readonly ISubjectPrerequisiteRepository _prerequisiteRepository;
        private CreateCurricularGridItemDto? _request;

        public CreateCurricularGridItemCommand(
            IDapperRlsContext dbContext,
            ICurricularGridRepository gridRepository,
            ISubjectPrerequisiteRepository prerequisiteRepository)
            : base(dbContext)
        {
            _gridRepository = gridRepository;
            _prerequisiteRepository = prerequisiteRepository;
        }

        public CreateCurricularGridItemCommand WithRequest(CreateCurricularGridItemDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            CurricularGridItemHelper.ValidateFields(
                null,
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
            var created = await _gridRepository.CreateAsync(
                new CurricularGridItemEntity
                {
                    AcademicPlanDetailId = dto.AcademicPlanDetailId,
                    SubjectId = dto.SubjectId,
                    TotalMeetings = dto.TotalMeetings,
                    MeetingsPerWeek = dto.MeetingsPerWeek,
                    SubjectHours = dto.SubjectHours,
                    Credits = dto.Credits,
                    IsMandatory = dto.IsMandatory,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            var prerequisiteIds = dto.PrerequisiteCurricularGridItemIds ?? [];
            if (prerequisiteIds.Count > 0)
            {
                await _prerequisiteRepository.ReplacePrerequisitesAsync(created.Id, prerequisiteIds, cancellationToken);
            }

            return CurricularGridItemHelper.MapToDto(created, prerequisiteIds);
        }
    }
}