using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Subjects
{
    public sealed class DeleteSubjectCommand : TransactionalCommandBase<bool>
    {
        private readonly ISubjectRepository _repository;
        private Guid _id;

        public DeleteSubjectCommand(IDapperRlsContext dbContext, ISubjectRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public DeleteSubjectCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Materia con id '{_id}' no fue encontrada.");
            }

            return true;
        }
    }
}
