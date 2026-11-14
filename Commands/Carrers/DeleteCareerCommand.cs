using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Carrers
{
    public sealed class DeleteCareerCommand : TransactionalCommandBase<bool>
    {
        private readonly ICareerRepository _careerRepository;
        private Guid _id;

        public DeleteCareerCommand(IDapperRlsContext dbContext, ICareerRepository careerRepository)
            : base(dbContext)
        {
            _careerRepository = careerRepository;
        }

        public DeleteCareerCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _careerRepository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Carrara con id '{_id}' no fue encontrada.");
            }

            return true;
        }
    }
}
