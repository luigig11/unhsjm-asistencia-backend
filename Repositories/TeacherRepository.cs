using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class TeacherRepository : ITeacherRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public TeacherRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<TeacherWithDetailsEntity>> GetAllAsync(
            TeacherFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var whereBuilder = new StringBuilder("WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                whereBuilder.Append("""
                 AND (
                    d.codigo_empleado ILIKE @Search
                    OR u.cedula ILIKE @Search
                    OR u.nombre ILIKE @Search
                    OR u.apellido ILIKE @Search
                    OR (u.nombre || ' ' || u.apellido) ILIKE @Search
                )
                """);
                parameters.Add("Search", $"%{filter.Search.Trim()}%");
            }

            if (filter.DepartmentId.HasValue)
            {
                whereBuilder.Append(" AND d.id_direccion = @DepartmentId");
                parameters.Add("DepartmentId", filter.DepartmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.ContractType))
            {
                whereBuilder.Append(" AND upper(d.tipo_contratacion::text) = upper(@ContractType)");
                parameters.Add("ContractType", filter.ContractType.Trim());
            }

            if (filter.IsActive.HasValue)
            {
                whereBuilder.Append(" AND d.activo = @IsActive");
                parameters.Add("IsActive", filter.IsActive.Value);
            }

            var sql = $"""
            SELECT
                d.id AS Id,
                d.id_usuario AS UserId,
                u.nombre AS FirstName,
                u.apellido AS LastName,
                u.cedula AS NationalId,
                u.genero::text AS Gender,
                d.id_direccion AS DepartmentId,
                dir.nombre AS DepartmentName,
                d.codigo_empleado AS EmployeeCode,
                d.telefono AS Phone,
                d.tipo_contratacion::text AS ContractType,
                d.activo AS IsActive
            FROM public.docente d
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            INNER JOIN public.direccion dir ON dir.id = d.id_direccion
            {whereBuilder}
            ORDER BY u.apellido ASC, u.nombre ASC;
            """;

            var items = await _dbContext.QueryAsync<TeacherWithDetailsEntity>(
                sql,
                parameters,
                cancellationToken: cancellationToken);

            return items.ToList();
        }

        public async Task<TeacherWithDetailsEntity?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                d.id AS Id,
                d.id_usuario AS UserId,
                u.nombre AS FirstName,
                u.apellido AS LastName,
                u.cedula AS NationalId,
                u.genero::text AS Gender,
                d.id_direccion AS DepartmentId,
                dir.nombre AS DepartmentName,
                d.codigo_empleado AS EmployeeCode,
                d.telefono AS Phone,
                d.tipo_contratacion::text AS ContractType,
                d.activo AS IsActive
            FROM public.docente d
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            INNER JOIN public.direccion dir ON dir.id = d.id_direccion
            WHERE d.id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<TeacherWithDetailsEntity>(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);
        }

        public async Task<TeacherEntity> CreateAsync(TeacherEntity teacher, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.docente (
                id_usuario,
                id_direccion,
                codigo_empleado,
                telefono,
                tipo_contratacion,
                activo
            )
            VALUES (
                @UserId,
                @DepartmentId,
                @EmployeeCode,
                @Phone,
                @ContractType::tipo_contratacion_docente,
                @IsActive
            )
            RETURNING
                id AS Id,
                id_usuario AS UserId,
                id_direccion AS DepartmentId,
                codigo_empleado AS EmployeeCode,
                telefono AS Phone,
                tipo_contratacion::text AS ContractType,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<TeacherEntity>(
                sql,
                new
                {
                    teacher.UserId,
                    teacher.DepartmentId,
                    teacher.EmployeeCode,
                    teacher.Phone,
                    teacher.ContractType,
                    teacher.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<TeacherEntity?> UpdateAsync(Guid id, TeacherEntity teacher, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.docente
            SET
                id_direccion = @DepartmentId,
                codigo_empleado = @EmployeeCode,
                telefono = @Phone,
                tipo_contratacion = @ContractType::tipo_contratacion_docente,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_usuario AS UserId,
                id_direccion AS DepartmentId,
                codigo_empleado AS EmployeeCode,
                telefono AS Phone,
                tipo_contratacion::text AS ContractType,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<TeacherEntity>(
                sql,
                new
                {
                    Id = id,
                    teacher.DepartmentId,
                    teacher.EmployeeCode,
                    teacher.Phone,
                    teacher.ContractType,
                    teacher.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.docente WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}
