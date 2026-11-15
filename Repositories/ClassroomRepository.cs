using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class ClassroomRepository : IClassroomRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public ClassroomRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ClassroomWithTypeEntity>> GetAllAsync(
            Guid? classroomTypeId,
            string? search,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                a.id AS Id,
                a.id_tipo_aula AS ClassroomTypeId,
                ta.nombre AS ClassroomTypeName,
                a.codigo AS Code,
                a.nombre AS Name,
                a.capacidad AS Capacity,
                a.ubicacion AS Location,
                a.activo AS IsActive
            FROM public.aula a
            INNER JOIN public.tipo_aula ta ON ta.id = a.id_tipo_aula
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (classroomTypeId.HasValue)
            {
                sqlBuilder.Append(" AND a.id_tipo_aula = @ClassroomTypeId");
                parameters.Add("ClassroomTypeId", classroomTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                sqlBuilder.Append(" AND (a.codigo ILIKE @Search OR a.nombre ILIKE @Search)");
                parameters.Add("Search", $"%{search.Trim()}%");
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND a.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY a.nombre ASC;");

            var items = await _dbContext.QueryAsync<ClassroomWithTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<ClassroomEntity> CreateAsync(ClassroomEntity classroom, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.aula (id_tipo_aula, codigo, nombre, capacidad, ubicacion, activo)
            VALUES (@ClassroomTypeId, @Code, @Name, @Capacity, @Location, @IsActive)
            RETURNING
                id AS Id,
                id_tipo_aula AS ClassroomTypeId,
                codigo AS Code,
                nombre AS Name,
                capacidad AS Capacity,
                ubicacion AS Location,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<ClassroomEntity>(
                sql,
                new
                {
                    classroom.ClassroomTypeId,
                    classroom.Code,
                    classroom.Name,
                    classroom.Capacity,
                    classroom.Location,
                    classroom.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<ClassroomEntity?> UpdateAsync(Guid id, ClassroomEntity classroom, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.aula
            SET
                id_tipo_aula = @ClassroomTypeId,
                codigo = @Code,
                nombre = @Name,
                capacidad = @Capacity,
                ubicacion = @Location,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_tipo_aula AS ClassroomTypeId,
                codigo AS Code,
                nombre AS Name,
                capacidad AS Capacity,
                ubicacion AS Location,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<ClassroomEntity>(
                sql,
                new
                {
                    Id = id,
                    classroom.ClassroomTypeId,
                    classroom.Code,
                    classroom.Name,
                    classroom.Capacity,
                    classroom.Location,
                    classroom.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.aula WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}