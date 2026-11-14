using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IReadOnlyList<DepartmentEntity>> GetAllAsync(string? search, bool? isActive, CancellationToken cancellationToken = default);
        Task<DepartmentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DepartmentEntity> CreateAsync(DepartmentEntity department, CancellationToken cancellationToken = default);
        Task<DepartmentEntity?> UpdateAsync(Guid id, DepartmentEntity department, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
