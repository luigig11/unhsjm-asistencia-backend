using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Departments;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;
using unhsjm_asistencia_backend.Commands.Abstractions;

namespace unhsjm_asistencia_backend.Commands.Departments
{
    public sealed class CreateDepartmentCommand : TransactionalCommandBase<DepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private CreateDepartmentDto? _request;

        public CreateDepartmentCommand(IDapperRlsContext dbContext, IDepartmentRepository departmentRepository)
            : base(dbContext)
        {
            _departmentRepository = departmentRepository;
        }

        public CreateDepartmentCommand WithRequest(CreateDepartmentDto request)
        {
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
            var created = await _departmentRepository.CreateAsync(
                new DepartmentEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListDepartmentsCommand.MapToDto(created);
        }
    }
}
