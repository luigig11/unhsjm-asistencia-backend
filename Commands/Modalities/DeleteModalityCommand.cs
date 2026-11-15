using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Modalities
{
    public sealed class DeleteModalityCommand : TransactionalCommandBase<bool>
    {
        private readonly IModalityRepository _modalityRepository;
        private Guid _id;

        public DeleteModalityCommand(IDapperRlsContext dbContext, IModalityRepository modalityRepository)
            : base(dbContext)
        {
            _modalityRepository = modalityRepository;
        }

        public DeleteModalityCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _modalityRepository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Modalidad con id '{_id}' no fue requerido.");
            }

            return true;
        }
    }
}
