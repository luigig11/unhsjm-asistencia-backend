using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.GroupEnrollments;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupEnrollments
{
    public sealed class CreateGroupEnrollmentCommand : TransactionalCommandBase<GroupEnrollmentDto>
    {
        private readonly IGroupEnrollmentRepository _repository;
        private CreateGroupEnrollmentDto? _request;

        public CreateGroupEnrollmentCommand(IDapperRlsContext dbContext, IGroupEnrollmentRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateGroupEnrollmentCommand WithRequest(CreateGroupEnrollmentDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            Validator.Create()
                .Field("id_materia_por_grupo", dto.GroupSubjectId, "grupo de materia").RequiredGuid()
                .Field("id_estudiante", dto.StudentId, "estudiante").RequiredGuid()
                .ValidateAndThrow();
        }

        protected override async Task<GroupEnrollmentDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;

            var courseQuota = await _repository.LockCourseAndCountActiveAsync(dto.GroupSubjectId, cancellationToken)
                ?? throw new NotFoundApiException($"Grupo de materia con id '{dto.GroupSubjectId}' no fue encontrado.");

            if (dto.IsActive && courseQuota.ActiveCount >= courseQuota.QuotaLimit)
            {
                throw new ConflictApiException(
                    $"Cupo del curso ({courseQuota.QuotaLimit}) ya fue completado.",
                    new Dictionary<string, string[]>
                    {
                        ["cupo_limite"] = [$"Cupo maximo de {courseQuota.QuotaLimit} estudianes activos ha sido alcanzado."]
                    });
            }

            var created = await _repository.CreateAsync(
                new GroupEnrollmentEntity
                {
                    GroupSubjectId = dto.GroupSubjectId,
                    StudentId = dto.StudentId,
                    EnrollmentDate = dto.EnrollmentDate ?? TimeZoneHelper.TodayInManagua(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListGroupEnrollmentsCommand.MapToDto(created);
        }
    }
}