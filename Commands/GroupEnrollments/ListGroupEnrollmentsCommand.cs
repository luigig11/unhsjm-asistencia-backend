using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.GroupEnrollments;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupEnrollments
{
    public sealed class ListGroupEnrollmentsCommand : CommandBase<IReadOnlyList<GroupEnrollmentListItemDto>>
    {
        private readonly IGroupEnrollmentRepository _repository;
        private Guid? _groupSubjectId;
        private Guid? _studentId;
        private bool? _isActive;

        public ListGroupEnrollmentsCommand(IGroupEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public ListGroupEnrollmentsCommand WithFilter(Guid? groupSubjectId, Guid? studentId, bool? isActive)
        {
            _groupSubjectId = groupSubjectId;
            _studentId = studentId;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<GroupEnrollmentListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_groupSubjectId, _studentId, _isActive, cancellationToken);
            return items
                .Select(entity => new GroupEnrollmentListItemDto
                {
                    Id = entity.Id,
                    GroupSubjectId = entity.GroupSubjectId,
                    StudentId = entity.StudentId,
                    UniqueCode = entity.UniqueCode,
                    StudentCarnet = entity.StudentCarnet,
                    FirstNames = entity.FirstNames,
                    FirstLastName = entity.FirstLastName,
                    SecondLastName = entity.SecondLastName,
                    EnrollmentDate = entity.EnrollmentDate,
                    IsActive = entity.IsActive
                })
                .ToList();
        }

        internal static GroupEnrollmentDto MapToDto(GroupEnrollmentEntity entity) => new()
        {
            Id = entity.Id,
            GroupSubjectId = entity.GroupSubjectId,
            StudentId = entity.StudentId,
            EnrollmentDate = entity.EnrollmentDate,
            IsActive = entity.IsActive
        };
    }
}