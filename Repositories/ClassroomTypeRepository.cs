using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class ClassroomTypeRepository : IClassroomTypeRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public ClassroomTypeRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ClassroomTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                descripcion AS Description,
                activo AS IsActive
            FROM public.tipo_aula
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY nombre ASC;");

            var items = await _dbContext.QueryAsync<ClassroomTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<ClassroomTypeEntity> CreateAsync(ClassroomTypeEntity classroomType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.tipo_aula (codigo, nombre, descripcion, activo)
            VALUES (@Code, @Name, @Description, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, descripcion AS Description, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<ClassroomTypeEntity>(
                sql,
                new { classroomType.Code, classroomType.Name, classroomType.Description, classroomType.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<ClassroomTypeEntity?> UpdateAsync(Guid id, ClassroomTypeEntity classroomType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.tipo_aula
            SET codigo = @Code, nombre = @Name, descripcion = @Description, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, descripcion AS Description, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<ClassroomTypeEntity>(
                sql,
                new { Id = id, classroomType.Code, classroomType.Name, classroomType.Description, classroomType.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.tipo_aula WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}