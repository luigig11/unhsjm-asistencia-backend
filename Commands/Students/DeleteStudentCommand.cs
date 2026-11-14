using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Students
{
    public sealed class DeleteStudentCommand : CommandBase<bool>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IStudentRepository _studentRepository;
        private Guid _studentId;

        public DeleteStudentCommand(IDapperRlsContext dbContext, IStudentRepository studentRepository)
        {
            _dbContext = dbContext;
            _studentRepository = studentRepository;
        }

        public DeleteStudentCommand WithId(Guid id)
        {
            _studentId = id;
            return this;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var deleted = await _studentRepository.DeleteAsync(_studentId, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Estudiante con id '{_studentId}' no fue encontrado.");
            }

            await _dbContext.CommitTransactionAsync(cancellationToken);
            return true;
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.HasActiveTransaction)
            {
                await _dbContext.RollbackTransactionAsync(cancellationToken);
            }
        }
    }
}
