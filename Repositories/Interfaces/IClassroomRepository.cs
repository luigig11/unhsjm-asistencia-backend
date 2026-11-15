using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IClassroomRepository
    {
        Task<IReadOnlyList<ClassroomWithTypeEntity>> GetAllAsync(Guid? classroomTypeId, string? search, bool? isActive, CancellationToken cancellationToken = default);
        Task<ClassroomEntity> CreateAsync(ClassroomEntity classroom, CancellationToken cancellationToken = default);
        Task<ClassroomEntity?> UpdateAsync(Guid id, ClassroomEntity classroom, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}