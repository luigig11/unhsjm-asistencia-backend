using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IReadOnlyList<TeacherWithDetailsEntity>> GetAllAsync(TeacherFilterDto filter, CancellationToken cancellationToken = default);
        Task<TeacherWithDetailsEntity?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TeacherEntity> CreateAsync(TeacherEntity teacher, CancellationToken cancellationToken = default);
        Task<TeacherEntity?> UpdateAsync(Guid id, TeacherEntity teacher, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
