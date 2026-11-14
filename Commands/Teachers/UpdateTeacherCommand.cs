using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Teachers
{
    public sealed class UpdateTeacherCommand : CommandBase<TeacherDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly ITeacherRepository _teacherRepository;
        private Guid _teacherId;
        private UpdateTeacherDto? _request;

        public UpdateTeacherCommand(IDapperRlsContext dbContext, ITeacherRepository teacherRepository)
        {
            _dbContext = dbContext;
            _teacherRepository = teacherRepository;
        }

        public UpdateTeacherCommand WithRequest(Guid id, UpdateTeacherDto request)
        {
            _teacherId = id;
            _request = request;
            return this;
        }

        public override async Task<TeacherDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ValidateRequest(dto.DepartmentId, dto.EmployeeCode, dto.Phone, dto.ContractType);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var updated = await _teacherRepository.UpdateAsync(
                _teacherId,
                new TeacherEntity
                {
                    Id = _teacherId,
                    DepartmentId = dto.DepartmentId,
                    EmployeeCode = dto.EmployeeCode.Trim(),
                    Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                    ContractType = dto.ContractType.Trim().ToUpperInvariant(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Maestro con id '{_teacherId}' no fue encontrado.");

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new TeacherDto
            {
                Id = updated.Id,
                UserId = updated.UserId,
                DepartmentId = updated.DepartmentId,
                EmployeeCode = updated.EmployeeCode,
                Phone = updated.Phone,
                ContractType = updated.ContractType,
                IsActive = updated.IsActive
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
            Guid departmentId,
            string? employeeCode,
            string? phone,
            string? contractType)
        {
            var errors = new Dictionary<string, string[]>();

            if (departmentId == Guid.Empty)
            {
                errors["id_direccion"] = ["Direccion con id valido es requerido."];
            }

            if (string.IsNullOrWhiteSpace(employeeCode) || employeeCode.Trim().Length > 30)
            {
                errors["codigo_empleado"] = ["Codigo de empleado es requerido y menor de 30 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(phone) && phone.Trim().Length > 20)
            {
                errors["telefono"] = ["Numero debe tener a lo sumo 20 caracteres."];
            }

            var normalizedContract = contractType?.Trim().ToUpperInvariant();
            if (normalizedContract is not ("FIJO" or "TEMPORAL"))
            {
                errors["tipo_contratacion"] = ["Tipo de contrato debe ser 'FIJO' o 'TEMPORAL'."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Ocurrieron uno o mas errores de validacion.", errors);
            }
        }
    }
}
