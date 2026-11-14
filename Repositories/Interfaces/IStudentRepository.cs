using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<(int Total, IReadOnlyList<StudentWithDetailsEntity> Items)> GetPagedAsync(StudentFilterDto filter, CancellationToken cancellationToken = default);
        Task<StudentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StudentEntity> CreateAsync(StudentEntity student, CancellationToken cancellationToken = default);
        Task<StudentEntity?> UpdateAsync(Guid id, StudentEntity student, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
