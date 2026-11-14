using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ICareerRepository
    {
        Task<IReadOnlyList<CareerWithDepartmentEntity>> GetAllAsync(string? search, Guid? departmentId, bool? isActive, CancellationToken cancellationToken = default);
        Task<CareerEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CareerEntity> CreateAsync(CareerEntity career, CancellationToken cancellationToken = default);
        Task<CareerEntity?> UpdateAsync(Guid id, CareerEntity career, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
