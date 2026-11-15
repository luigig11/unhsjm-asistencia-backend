using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Subjects;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Subjects
{
    public sealed class CreateSubjectCommand : TransactionalCommandBase<SubjectDto>
    {
        private readonly ISubjectRepository _repository;
        private CreateSubjectDto? _request;

        public CreateSubjectCommand(IDapperRlsContext dbContext, ISubjectRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateSubjectCommand WithRequest(CreateSubjectDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListSubjectsCommand.ValidateFields(dto.Code, dto.Name);
        }

        protected override async Task<SubjectDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new SubjectEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListSubjectsCommand.MapToDto(created);
        }
    }
}
