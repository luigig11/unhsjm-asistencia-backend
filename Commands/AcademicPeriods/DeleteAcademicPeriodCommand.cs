using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriods
{
    public sealed class DeleteAcademicPeriodCommand : TransactionalCommandBase<bool>
    {
        private readonly IAcademicPeriodRepository _repository;
        private Guid _id;

        public DeleteAcademicPeriodCommand(IDapperRlsContext dbContext, IAcademicPeriodRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public DeleteAcademicPeriodCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Periodo academico con id '{_id}' no fue encontrado.");
            }

            return true;
        }
    }
}
