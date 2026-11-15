using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IGroupEnrollmentRepository
    {
        Task<IReadOnlyList<GroupEnrollmentWithStudentEntity>> GetAllAsync(
            Guid? groupSubjectId,
            Guid? studentId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<GroupEnrollmentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<(short QuotaLimit, int ActiveCount)?> LockCourseAndCountActiveAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default);

        Task<GroupEnrollmentEntity> CreateAsync(GroupEnrollmentEntity enrollment, CancellationToken cancellationToken = default);

        Task<GroupEnrollmentEntity?> UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}