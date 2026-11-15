using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.GroupEnrollments;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupEnrollments
{
    public sealed class UpdateGroupEnrollmentCommand : TransactionalCommandBase<GroupEnrollmentDto>
    {
        private readonly IGroupEnrollmentRepository _repository;
        private Guid _id;
        private UpdateGroupEnrollmentDto? _request;

        public UpdateGroupEnrollmentCommand(IDapperRlsContext dbContext, IGroupEnrollmentRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateGroupEnrollmentCommand WithRequest(Guid id, UpdateGroupEnrollmentDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override async Task<GroupEnrollmentDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload no fue encontrado.");

            var existing = await _repository.GetByIdAsync(_id, cancellationToken)
                ?? throw new NotFoundApiException($"Inscripcion de grupo con id '{_id}' no fue encontrado.");

            if (!existing.IsActive && dto.IsActive)
            {
                var courseQuota = await _repository.LockCourseAndCountActiveAsync(existing.GroupSubjectId, cancellationToken)
                    ?? throw new NotFoundApiException($"Grupo de materia con id '{existing.GroupSubjectId}' no fue encontrado.");

                if (courseQuota.ActiveCount >= courseQuota.QuotaLimit)
                {
                    throw new ConflictApiException(
                        $"Cupo limite de ({courseQuota.QuotaLimit}) ha sido alcanzadi.",
                        new Dictionary<string, string[]>
                        {
                            ["cupo_limite"] = [$"Cupo maximo de {courseQuota.QuotaLimit} estudiantes activos ha sido alanzado."]
                        });
                }
            }

            var updated = await _repository.UpdateStatusAsync(_id, dto.IsActive, cancellationToken)
                ?? throw new NotFoundApiException($"Inscripcion de grupo con id '{_id}' no fue encontrado.");

            return ListGroupEnrollmentsCommand.MapToDto(updated);
        }
    }
}