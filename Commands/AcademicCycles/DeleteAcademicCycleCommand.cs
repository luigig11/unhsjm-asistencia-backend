using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicCycles
{
    public sealed class DeleteAcademicCycleCommand : TransactionalCommandBase<bool>
    {
        private readonly IAcademicCycleRepository _repository;
        private Guid _id;

        public DeleteAcademicCycleCommand(IDapperRlsContext dbContext, IAcademicCycleRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public DeleteAcademicCycleCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Ciclo academico con id '{_id}' no fue encontrado.");
            }

            return true;
        }
    }
}
