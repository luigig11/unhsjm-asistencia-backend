using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Departments
{
    public sealed class DeleteDepartmentCommand : TransactionalCommandBase<bool>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private Guid _id;

        public DeleteDepartmentCommand(IDapperRlsContext dbContext, IDepartmentRepository departmentRepository)
            : base(dbContext)
        {
            _departmentRepository = departmentRepository;
        }

        public DeleteDepartmentCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        protected override async Task<bool> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var deleted = await _departmentRepository.DeleteAsync(_id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Direccion con id '{_id}' no fue encontrada.");
            }

            return true;
        }
    }
}
