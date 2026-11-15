using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Subjects;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Subjects
{
    public sealed class UpdateSubjectCommand : TransactionalCommandBase<SubjectDto>
    {
        private readonly ISubjectRepository _repository;
        private Guid _id;
        private UpdateSubjectDto? _request;

        public UpdateSubjectCommand(IDapperRlsContext dbContext, ISubjectRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateSubjectCommand WithRequest(Guid id, UpdateSubjectDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new SubjectEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Materia con id '{_id}' no fue encontrado.");

            return ListSubjectsCommand.MapToDto(updated);
        }
    }
}
