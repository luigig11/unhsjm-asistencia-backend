using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Departments;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Departments
{
    public sealed class UpdateDepartmentCommand : TransactionalCommandBase<DepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private Guid _id;
        private UpdateDepartmentDto? _request;

        public UpdateDepartmentCommand(IDapperRlsContext dbContext, IDepartmentRepository departmentRepository)
            : base(dbContext)
        {
            _departmentRepository = departmentRepository;
        }

        public UpdateDepartmentCommand WithRequest(Guid id, UpdateDepartmentDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListDepartmentsCommand.ValidateFields(dto.Code, dto.Name);
        }

        protected override async Task<DepartmentDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _departmentRepository.UpdateAsync(
                _id,
                new DepartmentEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Direccion con id '{_id}' no fue encotrada.");

            return ListDepartmentsCommand.MapToDto(updated);
        }
    }
}
