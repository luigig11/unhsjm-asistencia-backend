using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.SubjectPrerequisites
{
    public sealed class DeleteSubjectPrerequisiteCommand : TransactionalCommandBase<bool>
    {
        private readonly ISubjectPrerequisiteRepository _repository;
        private Guid _id;

        public DeleteSubjectPrerequisiteCommand(IDapperRlsContext dbContext, ISubjectPrerequisiteRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public DeleteSubjectPrerequisiteCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Materia prerequisito con id '{_id}' no fue encontrada.");
            }

            return true;
        }
    }
}