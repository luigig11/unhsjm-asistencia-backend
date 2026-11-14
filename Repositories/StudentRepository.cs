using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public StudentRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(int Total, IReadOnlyList<StudentWithDetailsEntity> Items)> GetPagedAsync(
            StudentFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var whereBuilder = new StringBuilder("WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                whereBuilder.Append("""
                 AND (
                    e.codigo_unico ILIKE @Search
                    OR e.carnet ILIKE @Search
                    OR e.cedula ILIKE @Search
                    OR e.nombres ILIKE @Search
                    OR e.primer_apellido ILIKE @Search
                    OR COALESCE(e.segundo_apellido, '') ILIKE @Search
                    OR (e.nombres || ' ' || e.primer_apellido || ' ' || COALESCE(e.segundo_apellido, '')) ILIKE @Search
                )
                """);
                parameters.Add("Search", $"%{filter.Search.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.UniqueCode))
            {
                whereBuilder.Append(" AND e.codigo_unico ILIKE @UniqueCode");
                parameters.Add("UniqueCode", $"%{filter.UniqueCode.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.StudentCarnet))
            {
                whereBuilder.Append(" AND e.carnet ILIKE @StudentCarnet");
                parameters.Add("StudentCarnet", $"%{filter.StudentCarnet.Trim()}%");
            }

            if (filter.CareerId.HasValue)
            {
                whereBuilder.Append(" AND e.id_carrera = @CareerId");
                parameters.Add("CareerId", filter.CareerId.Value);
            }

            if (filter.AcademicPlanId.HasValue)
            {
                whereBuilder.Append(" AND e.id_plan_academico = @AcademicPlanId");
                parameters.Add("AcademicPlanId", filter.AcademicPlanId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                whereBuilder.Append(" AND e.activo = @IsActive");
                parameters.Add("IsActive", filter.IsActive.Value);
            }

            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);
            var offset = (page - 1) * pageSize;

            parameters.Add("Limit", pageSize);
            parameters.Add("Offset", offset);

            var countSql = $"""
            SELECT COUNT(1)
            FROM public.estudiante e
            INNER JOIN public.carrera c ON c.id = e.id_carrera
            INNER JOIN public.plan_academico pa ON pa.id = e.id_plan_academico
            {whereBuilder};
            """;

            var itemsSql = $"""
            SELECT
                e.id AS Id,
                e.codigo_unico AS UniqueCode,
                e.carnet AS StudentCarnet,
                e.cedula AS NationalId,
                e.nombres AS FirstNames,
                e.primer_apellido AS FirstLastName,
                e.segundo_apellido AS SecondLastName,
                e.correo AS Email,
                e.telefono AS Phone,
                e.fecha_ingreso AS AdmissionDate,
                e.id_carrera AS CareerId,
                c.nombre AS CareerName,
                e.id_plan_academico AS AcademicPlanId,
                pa.codigo AS AcademicPlanCode,
                e.activo AS IsActive
            FROM public.estudiante e
            INNER JOIN public.carrera c ON c.id = e.id_carrera
            INNER JOIN public.plan_academico pa ON pa.id = e.id_plan_academico
            {whereBuilder}
            ORDER BY e.primer_apellido ASC, e.segundo_apellido ASC NULLS LAST, e.nombres ASC
            LIMIT @Limit OFFSET @Offset;
            """;

            var total = await _dbContext.ExecuteScalarAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
            var items = await _dbContext.QueryAsync<StudentWithDetailsEntity>(itemsSql, parameters, cancellationToken: cancellationToken);

            return (total, items.ToList());
        }

        public async Task<StudentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                codigo_unico AS UniqueCode,
                id_carrera AS CareerId,
                id_plan_academico AS AcademicPlanId,
                carnet AS StudentCarnet,
                cedula AS NationalId,
                nombres AS FirstNames,
                primer_apellido AS FirstLastName,
                segundo_apellido AS SecondLastName,
                correo AS Email,
                telefono AS Phone,
                fecha_ingreso AS AdmissionDate,
                activo AS IsActive
            FROM public.estudiante
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<StudentEntity>(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);
        }

        public async Task<StudentEntity> CreateAsync(StudentEntity student, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.estudiante (
                codigo_unico,
                id_carrera,
                id_plan_academico,
                carnet,
                cedula,
                nombres,
                primer_apellido,
                segundo_apellido,
                correo,
                telefono,
                fecha_ingreso,
                activo
            )
            VALUES (
                @UniqueCode,
                @CareerId,
                @AcademicPlanId,
                @StudentCarnet,
                @NationalId,
                @FirstNames,
                @FirstLastName,
                @SecondLastName,
                @Email,
                @Phone,
                @AdmissionDate,
                @IsActive
            )
            RETURNING
                id AS Id,
                codigo_unico AS UniqueCode,
                id_carrera AS CareerId,
                id_plan_academico AS AcademicPlanId,
                carnet AS StudentCarnet,
                cedula AS NationalId,
                nombres AS FirstNames,
                primer_apellido AS FirstLastName,
                segundo_apellido AS SecondLastName,
                correo AS Email,
                telefono AS Phone,
                fecha_ingreso AS AdmissionDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<StudentEntity>(
                sql,
                new
                {
                    student.UniqueCode,
                    student.CareerId,
                    student.AcademicPlanId,
                    student.StudentCarnet,
                    student.NationalId,
                    student.FirstNames,
                    student.FirstLastName,
                    student.SecondLastName,
                    student.Email,
                    student.Phone,
                    student.AdmissionDate,
                    student.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<StudentEntity?> UpdateAsync(Guid id, StudentEntity student, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.estudiante
            SET
                codigo_unico = @UniqueCode,
                id_carrera = @CareerId,
                id_plan_academico = @AcademicPlanId,
                carnet = @StudentCarnet,
                cedula = @NationalId,
                nombres = @FirstNames,
                primer_apellido = @FirstLastName,
                segundo_apellido = @SecondLastName,
                correo = @Email,
                telefono = @Phone,
                fecha_ingreso = @AdmissionDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                codigo_unico AS UniqueCode,
                id_carrera AS CareerId,
                id_plan_academico AS AcademicPlanId,
                carnet AS StudentCarnet,
                cedula AS NationalId,
                nombres AS FirstNames,
                primer_apellido AS FirstLastName,
                segundo_apellido AS SecondLastName,
                correo AS Email,
                telefono AS Phone,
                fecha_ingreso AS AdmissionDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<StudentEntity>(
                sql,
                new
                {
                    Id = id,
                    student.UniqueCode,
                    student.CareerId,
                    student.AcademicPlanId,
                    student.StudentCarnet,
                    student.NationalId,
                    student.FirstNames,
                    student.FirstLastName,
                    student.SecondLastName,
                    student.Email,
                    student.Phone,
                    student.AdmissionDate,
                    student.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.estudiante WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}
