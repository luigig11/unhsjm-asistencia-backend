using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class WeeklyScheduleRepository : IWeeklyScheduleRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public WeeklyScheduleRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<WeeklyScheduleWithDetailsEntity>> GetAllAsync(
            Guid? groupSubjectId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                cs.id AS Id,
                cs.id_materia_por_grupo AS GroupSubjectId,
                cs.id_aula AS ClassroomId,
                a.codigo AS ClassroomCode,
                a.nombre AS ClassroomName,
                cs.id_dia AS DayId,
                d.nombre AS DayName,
                cs.hora_inicio AS StartTime,
                cs.hora_fin AS EndTime,
                cs.activo AS IsActive
            FROM public.calendario_semanal cs
            INNER JOIN public.aula a ON a.id = cs.id_aula
            INNER JOIN public.dia d ON d.id = cs.id_dia
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (groupSubjectId.HasValue)
            {
                sqlBuilder.Append(" AND cs.id_materia_por_grupo = @GroupSubjectId");
                parameters.Add("GroupSubjectId", groupSubjectId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND cs.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY cs.id_dia ASC, cs.hora_inicio ASC;");

            var items = await _dbContext.QueryAsync<WeeklyScheduleWithDetailsEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<WeeklyScheduleEntity> CreateAsync(WeeklyScheduleEntity schedule, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.calendario_semanal (id_materia_por_grupo, id_aula, id_dia, hora_inicio, hora_fin, activo)
            VALUES (@GroupSubjectId, @ClassroomId, @DayId, @StartTime, @EndTime, @IsActive)
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_aula AS ClassroomId,
                id_dia AS DayId,
                hora_inicio AS StartTime,
                hora_fin AS EndTime,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<WeeklyScheduleEntity>(sql, schedule, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<WeeklyScheduleEntity?> UpdateAsync(
            Guid id,
            Guid classroomId,
            short dayId,
            TimeOnly startTime,
            TimeOnly endTime,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.calendario_semanal
            SET
                id_aula = @ClassroomId,
                id_dia = @DayId,
                hora_inicio = @StartTime,
                hora_fin = @EndTime,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_aula AS ClassroomId,
                id_dia AS DayId,
                hora_inicio AS StartTime,
                hora_fin AS EndTime,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<WeeklyScheduleEntity>(
                sql,
                new
                {
                    Id = id,
                    ClassroomId = classroomId,
                    DayId = dayId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = isActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.calendario_semanal WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}