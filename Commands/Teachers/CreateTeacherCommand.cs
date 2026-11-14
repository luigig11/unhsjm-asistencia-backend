using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;
using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Commands.Teachers
{
    public sealed class CreateTeacherCommand : CommandBase<TeacherDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly ITeacherRepository _teacherRepository;
        private CreateTeacherDto? _request;

        public CreateTeacherCommand(IDapperRlsContext dbContext, ITeacherRepository teacherRepository)
        {
            _dbContext = dbContext;
            _teacherRepository = teacherRepository;
        }

        public CreateTeacherCommand WithRequest(CreateTeacherDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<TeacherDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ValidateRequest(dto.UserId, dto.DepartmentId, dto.EmployeeCode, dto.Phone, dto.ContractType);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var created = await _teacherRepository.CreateAsync(
                new TeacherEntity
                {
                    UserId = dto.UserId,
                    DepartmentId = dto.DepartmentId,
                    EmployeeCode = dto.EmployeeCode.Trim(),
                    Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                    ContractType = dto.ContractType.Trim().ToUpperInvariant(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new TeacherDto
            {
                Id = created.Id,
                UserId = created.UserId,
                DepartmentId = created.DepartmentId,
                EmployeeCode = created.EmployeeCode,
                Phone = created.Phone,
                ContractType = created.ContractType,
                IsActive = created.IsActive
            };
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.HasActiveTransaction)
            {
                await _dbContext.RollbackTransactionAsync(cancellationToken);
            }
        }

        private static void ValidateRequest(
            Guid userId,
            Guid departmentId,
            string? employeeCode,
            string? phone,
            string? contractType)
        {
            var errors = new Dictionary<string, string[]>();

            if (userId == Guid.Empty)
            {
                errors["id_usuario"] = ["Usuario con id valido es requerido."];
            }

            if (departmentId == Guid.Empty)
            {
                errors["id_direccion"] = ["Direccion con id valido es requerido."];
            }

            if (string.IsNullOrWhiteSpace(employeeCode) || employeeCode.Trim().Length > 30)
            {
                errors["codigo_empleado"] = ["Codigo de empleado es requerido y menor a 30 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(phone) && phone.Trim().Length > 20)
            {
                errors["telefono"] = ["Numero de telefono debe tener 20 caracteres maximo."];
            }

            var normalizedContract = contractType?.Trim().ToUpperInvariant();
            if (normalizedContract is not ("FIJO" or "TEMPORAL"))
            {
                errors["tipo_contratacion"] = ["Tipo de contrato debe ser 'FIJO' o 'TEMPORAL'."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Una o mas validaciones fallaron.", errors);
            }
        }
    }
}
