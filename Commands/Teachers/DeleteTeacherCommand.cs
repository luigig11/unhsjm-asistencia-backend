using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Teachers
{
    public sealed class DeleteTeacherCommand : CommandBase<bool>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly ITeacherRepository _teacherRepository;
        private Guid _teacherId;

        public DeleteTeacherCommand(IDapperRlsContext dbContext, ITeacherRepository teacherRepository)
        {
            _dbContext = dbContext;
            _teacherRepository = teacherRepository;
        }

        public DeleteTeacherCommand WithId(Guid id)
        {
            _teacherId = id;
            return this;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var deleted = await _teacherRepository.DeleteAsync(_teacherId, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Maestro con id '{_teacherId}' no fue encontrado.");
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
