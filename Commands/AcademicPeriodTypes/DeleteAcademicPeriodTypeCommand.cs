using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriodTypes
{
    public sealed class DeleteAcademicPeriodTypeCommand : TransactionalCommandBase<bool>
    {
        private readonly IAcademicPeriodTypeRepository _repository;
        private Guid _id;

        public DeleteAcademicPeriodTypeCommand(IDapperRlsContext dbContext, IAcademicPeriodTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public DeleteAcademicPeriodTypeCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Tipo de periodo academico con id '{_id}' no fue encontrado.");
            }

            return true;
        }
    }
}
