using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ISchoolPeriodRepository
    {
        Task<IReadOnlyList<SchoolPeriodEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);
        Task<SchoolPeriodEntity> CreateAsync(SchoolPeriodEntity period, CancellationToken cancellationToken = default);
        Task<SchoolPeriodEntity?> UpdateAsync(Guid id, SchoolPeriodEntity period, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}