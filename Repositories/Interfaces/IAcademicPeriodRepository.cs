using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicPeriodRepository
    {
        Task<IReadOnlyList<AcademicPeriodWithTypeEntity>> GetAllAsync(Guid? academicPeriodTypeId, bool? isActive, CancellationToken cancellationToken = default);
        Task<AcademicPeriodEntity> CreateAsync(AcademicPeriodEntity period, CancellationToken cancellationToken = default);
        Task<AcademicPeriodEntity?> UpdateAsync(Guid id, AcademicPeriodEntity period, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}



