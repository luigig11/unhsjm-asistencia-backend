using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.GroupSubjects;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupSubjects
{
    public sealed class UpdateGroupSubjectCommand : TransactionalCommandBase<GroupSubjectDto>
    {
        private readonly IGroupSubjectRepository _repository;
        private Guid _id;
        private UpdateGroupSubjectDto? _request;

        public UpdateGroupSubjectCommand(IDapperRlsContext dbContext, IGroupSubjectRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateGroupSubjectCommand WithRequest(Guid id, UpdateGroupSubjectDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            Validator.Create()
                .Field("id_docente", dto.TeacherId, "maestri").RequiredGuid()
                .Field("cupo_limite", dto.QuotaLimit, "cupo limite").Positive()
                .ValidateAndThrow();
        }

        protected override async Task<GroupSubjectDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(_id, dto.TeacherId, dto.QuotaLimit, dto.IsActive, cancellationToken)
                ?? throw new NotFoundApiException($"Grupo de materia con ia '{_id}' no fue encontrado.");

            return ListGroupSubjectsCommand.MapToDto(updated);
        }
    }
}