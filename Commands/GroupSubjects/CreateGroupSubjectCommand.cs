using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.GroupSubjects;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.GroupSubjects
{
    public sealed class CreateGroupSubjectCommand : TransactionalCommandBase<GroupSubjectDto>
    {
        private readonly IGroupSubjectRepository _repository;
        private CreateGroupSubjectDto? _request;

        public CreateGroupSubjectCommand(IDapperRlsContext dbContext, IGroupSubjectRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateGroupSubjectCommand WithRequest(CreateGroupSubjectDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            Validator.Create()
                .Field("id_grupo", dto.GroupId, "grupo").RequiredGuid()
                .Field("id_malla_curricular", dto.CurricularGridItemId, "item de malla curricular").RequiredGuid()
                .Field("id_docente", dto.TeacherId, "maestro").RequiredGuid()
                .Field("cupo_limite", dto.QuotaLimit, "cupo limite").Positive()
                .ValidateAndThrow();
        }

        protected override async Task<GroupSubjectDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new GroupSubjectEntity
                {
                    GroupId = dto.GroupId,
                    CurricularGridItemId = dto.CurricularGridItemId,
                    TeacherId = dto.TeacherId,
                    QuotaLimit = dto.QuotaLimit,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListGroupSubjectsCommand.MapToDto(created);
        }
    }
}