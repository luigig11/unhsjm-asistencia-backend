using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.GroupSubjects;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupSubjects
{
    public sealed class ListGroupSubjectsCommand : CommandBase<IReadOnlyList<GroupSubjectListItemDto>>
    {
        private readonly IGroupSubjectRepository _repository;
        private Guid? _groupId;
        private Guid? _teacherId;
        private bool? _isActive;

        public ListGroupSubjectsCommand(IGroupSubjectRepository repository)
        {
            _repository = repository;
        }

        public ListGroupSubjectsCommand WithFilter(Guid? groupId, Guid? teacherId, bool? isActive)
        {
            _groupId = groupId;
            _teacherId = teacherId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<GroupSubjectListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_groupId, _teacherId, _isActive, cancellationToken);
            return items
                .Select(entity => new GroupSubjectListItemDto
                {
                    Id = entity.Id,
                    GroupId = entity.GroupId,
                    GroupCode = entity.GroupCode,
                    CurricularGridItemId = entity.CurricularGridItemId,
                    SubjectId = entity.SubjectId,
                    SubjectCode = entity.SubjectCode,
                    SubjectName = entity.SubjectName,
                    TeacherId = entity.TeacherId,
                    TeacherFullName = entity.TeacherFullName,
                    QuotaLimit = entity.QuotaLimit,
                    TotalEnrolled = entity.TotalEnrolled,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static GroupSubjectDto MapToDto(GroupSubjectEntity entity) => new()
        {
            Id = entity.Id,
            GroupId = entity.GroupId,
            CurricularGridItemId = entity.CurricularGridItemId,
            TeacherId = entity.TeacherId,
            QuotaLimit = entity.QuotaLimit,
            IsActive = entity.IsActive
        };
    }
}