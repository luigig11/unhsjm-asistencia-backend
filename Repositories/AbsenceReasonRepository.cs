using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AbsenceReasonRepository : IAbsenceReasonRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AbsenceReasonRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AbsenceReasonEntity>> GetAllAsync(
            string? appliesTo,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                descripcion AS Description,
                justifica_inasistencia AS JustifiesAbsence,
                aplica_a AS AppliesTo,
                activo AS IsActive
            FROM public.motivo_ausencia
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(appliesTo))
            {
                sqlBuilder.Append(" AND upper(aplica_a) = upper(@AppliesTo)");
                parameters.Add("AppliesTo", appliesTo.Trim());
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY nombre ASC;");

            var items = await _dbContext.QueryAsync<AbsenceReasonEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AbsenceReasonEntity> CreateAsync(AbsenceReasonEntity absenceReason, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.motivo_ausencia (
                codigo, nombre, descripcion, justifica_inasistencia, aplica_a, activo
            )
            VALUES (
                @Code, @Name, @Description, @JustifiesAbsence, @AppliesTo, @IsActive
            )
            RETURNING
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                descripcion AS Description,
                justifica_inasistencia AS JustifiesAbsence,
                aplica_a AS AppliesTo,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AbsenceReasonEntity>(
                sql,
                new
                {
                    absenceReason.Code,
                    absenceReason.Name,
                    absenceReason.Description,
                    absenceReason.JustifiesAbsence,
                    absenceReason.AppliesTo,
                    absenceReason.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<AbsenceReasonEntity?> UpdateAsync(Guid id, AbsenceReasonEntity absenceReason, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.motivo_ausencia
            SET
                codigo = @Code,
                nombre = @Name,
                descripcion = @Description,
                justifica_inasistencia = @JustifiesAbsence,
                aplica_a = @AppliesTo,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                descripcion AS Description,
                justifica_inasistencia AS JustifiesAbsence,
                aplica_a AS AppliesTo,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AbsenceReasonEntity>(
                sql,
                new
                {
                    Id = id,
                    absenceReason.Code,
                    absenceReason.Name,
                    absenceReason.Description,
                    absenceReason.JustifiesAbsence,
                    absenceReason.AppliesTo,
                    absenceReason.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.motivo_ausencia WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}