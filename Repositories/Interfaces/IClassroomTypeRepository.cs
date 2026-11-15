using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IClassroomTypeRepository
    {
        Task<IReadOnlyList<ClassroomTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);
        Task<ClassroomTypeEntity> CreateAsync(ClassroomTypeEntity classroomType, CancellationToken cancellationToken = default);
        Task<ClassroomTypeEntity?> UpdateAsync(Guid id, ClassroomTypeEntity classroomType, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}