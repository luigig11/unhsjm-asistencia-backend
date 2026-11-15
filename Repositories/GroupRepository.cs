using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class GroupRepository : IGroupRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public GroupRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<GroupWithInstanceEntity>> GetAllAsync(
            Guid? academicInstanceId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                g.id AS Id,
                g.id_instancia_academica AS AcademicInstanceId,
                ia.nombre AS InstanceName,
                g.codigo AS Code,
                g.nombre AS Name,
                g.cupo AS Capacity,
                g.activo AS IsActive
            FROM public.grupo g
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (academicInstanceId.HasValue)
            {
                sqlBuilder.Append(" AND g.id_instancia_academica = @AcademicInstanceId");
                parameters.Add("AcademicInstanceId", academicInstanceId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND g.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY g.codigo ASC;");

            var items = await _dbContext.QueryAsync<GroupWithInstanceEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<GroupEntity> CreateAsync(GroupEntity group, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.grupo (id_instancia_academica, codigo, nombre, cupo, activo)
            VALUES (@AcademicInstanceId, @Code, @Name, @Capacity, @IsActive)
            RETURNING
                id AS Id,
                id_instancia_academica AS AcademicInstanceId,
                codigo AS Code,
                nombre AS Name,
                cupo AS Capacity,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<GroupEntity>(sql, group, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<GroupEntity?> UpdateAsync(Guid id, GroupEntity group, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.grupo
            SET
                id_instancia_academica = @AcademicInstanceId,
                codigo = @Code,
                nombre = @Name,
                cupo = @Capacity,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_instancia_academica AS AcademicInstanceId,
                codigo AS Code,
                nombre AS Name,
                cupo AS Capacity,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<GroupEntity>(
                sql,
                new { Id = id, group.AcademicInstanceId, group.Code, group.Name, group.Capacity, group.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.grupo WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}