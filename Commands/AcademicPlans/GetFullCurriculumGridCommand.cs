using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlans
{
    public sealed class GetFullCurriculumGridCommand : CommandBase<FullCurriculumGridDto>
    {
        private readonly IAcademicPlanRepository _planRepository;
        private readonly IAcademicPlanDetailRepository _detailRepository;
        private readonly ICurricularGridRepository _gridRepository;
        private readonly ISubjectPrerequisiteRepository _prerequisiteRepository;
        private Guid _planId;

        public GetFullCurriculumGridCommand(
            IAcademicPlanRepository planRepository,
            IAcademicPlanDetailRepository detailRepository,
            ICurricularGridRepository gridRepository,
            ISubjectPrerequisiteRepository prerequisiteRepository)
        {
            _planRepository = planRepository;
            _detailRepository = detailRepository;
            _gridRepository = gridRepository;
            _prerequisiteRepository = prerequisiteRepository;
        }

        public GetFullCurriculumGridCommand WithPlanId(Guid planId)
        {
            _planId = planId;
            return this;
        }

        public override async Task<FullCurriculumGridDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var plan = await _planRepository.GetDetailedByIdAsync(_planId, cancellationToken)
                ?? throw new NotFoundApiException($"Plan academico con id '{_planId}' no fue encontrado.");

            var details = await _detailRepository.GetAllAsync(_planId, isActive: null, cancellationToken);
            var gridItems = await _gridRepository.GetByPlanIdAsync(_planId, cancellationToken);
            var prerequisites = await _prerequisiteRepository.GetByPlanIdAsync(_planId, cancellationToken);

            var prerequisitesByGridItem = prerequisites
                .GroupBy(p => p.CurricularGridItemId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<CurriculumPrerequisiteNodeDto>)g
                        .Select(p => new CurriculumPrerequisiteNodeDto
                        {
                            Id = p.Id,
                            PrerequisiteCurricularGridItemId = p.PrerequisiteCurricularGridItemId,
                            SubjectId = p.PrerequisiteSubjectId,
                            SubjectCode = p.PrerequisiteSubjectCode,
                            SubjectName = p.PrerequisiteSubjectName,
                            IsActive = p.IsActive
                        })
                        .ToList());

            var subjectsByDetail = gridItems
                .GroupBy(item => item.AcademicPlanDetailId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<CurriculumSubjectNodeDto>)g
                        .Select(item => new CurriculumSubjectNodeDto
                        {
                            CurricularGridItemId = item.Id,
                            SubjectId = item.SubjectId,
                            SubjectCode = item.SubjectCode,
                            SubjectName = item.SubjectName,
                            TotalMeetings = item.TotalMeetings,
                            MeetingsPerWeek = item.MeetingsPerWeek,
                            SubjectHours = item.SubjectHours,
                            Credits = item.Credits,
                            IsMandatory = item.IsMandatory,
                            IsActive = item.IsActive,
                            Prerequisites = prerequisitesByGridItem.TryGetValue(item.Id, out var prereqList)
                                ? prereqList
                                : []
                        })
                        .ToList());

            var cycles = details
                .GroupBy(d => new { d.AcademicCycleId, d.AcademicCycleCode, d.AcademicCycleName, d.AcademicCycleOrder })
                .OrderBy(g => g.Key.AcademicCycleOrder)
                .Select(cycleGroup => new CurriculumCycleNodeDto
                {
                    AcademicCycleId = cycleGroup.Key.AcademicCycleId,
                    Code = cycleGroup.Key.AcademicCycleCode,
                    Name = cycleGroup.Key.AcademicCycleName,
                    Order = cycleGroup.Key.AcademicCycleOrder,
                    Blocks = cycleGroup
                        .OrderBy(b => b.AcademicPeriodOrder)
                        .Select(block => new CurriculumBlockNodeDto
                        {
                            AcademicPlanDetailId = block.Id,
                            AcademicPeriodId = block.AcademicPeriodId,
                            PeriodCode = block.AcademicPeriodCode,
                            PeriodName = block.AcademicPeriodName,
                            PeriodOrder = block.AcademicPeriodOrder,
                            DurationWeeks = block.DurationWeeks,
                            DurationMonths = block.DurationMonths,
                            PredecessorDetailId = block.PredecessorDetailId,
                            IsActive = block.IsActive,
                            Subjects = subjectsByDetail.TryGetValue(block.Id, out var subjectList)
                                ? subjectList
                                : []
                        })
                        .ToList()
                })
                .ToList();

            return new FullCurriculumGridDto
            {
                Plan = ListAcademicPlansCommand.MapToListItemDto(plan),
                Cycles = cycles
            };
        }
    }
}