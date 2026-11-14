using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Departments;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Departments
{
    public sealed class GetDepartmentByIdCommand : CommandBase<DepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private Guid _id;

        public GetDepartmentByIdCommand(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public GetDepartmentByIdCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public override async Task<DepartmentDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var entity = await _departmentRepository.GetByIdAsync(_id, cancellationToken)
                ?? throw new NotFoundApiException($"Department with id '{_id}' was not found.");

            return ListDepartmentsCommand.MapToDto(entity);
        }
    }
}
