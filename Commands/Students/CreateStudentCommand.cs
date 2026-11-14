using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Students
{
    public sealed class CreateStudentCommand : CommandBase<StudentDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IStudentRepository _studentRepository;
        private CreateStudentDto? _request;

        public CreateStudentCommand(IDapperRlsContext dbContext, IStudentRepository studentRepository)
        {
            _dbContext = dbContext;
            _studentRepository = studentRepository;
        }

        public CreateStudentCommand WithRequest(CreateStudentDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<StudentDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ValidateStudentFields(
                dto.UniqueCode,
                dto.CareerId,
                dto.AcademicPlanId,
                dto.StudentCarnet,
                dto.NationalId,
                dto.FirstNames,
                dto.FirstLastName,
                dto.SecondLastName,
                dto.Email,
                dto.Phone);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var created = await _studentRepository.CreateAsync(
                new StudentEntity
                {
                    UniqueCode = dto.UniqueCode.Trim(),
                    CareerId = dto.CareerId,
                    AcademicPlanId = dto.AcademicPlanId,
                    StudentCarnet = dto.StudentCarnet.Trim(),
                    NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim(),
                    FirstNames = dto.FirstNames.Trim(),
                    FirstLastName = dto.FirstLastName.Trim(),
                    SecondLastName = string.IsNullOrWhiteSpace(dto.SecondLastName) ? null : dto.SecondLastName.Trim(),
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                    AdmissionDate = dto.AdmissionDate ?? TimeZoneHelper.TodayInManagua(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new StudentDto
            {
                Id = created.Id,
                UniqueCode = created.UniqueCode,
                CareerId = created.CareerId,
                AcademicPlanId = created.AcademicPlanId,
                StudentCarnet = created.StudentCarnet,
                NationalId = created.NationalId,
                FirstNames = created.FirstNames,
                FirstLastName = created.FirstLastName,
                SecondLastName = created.SecondLastName,
                Email = created.Email,
                Phone = created.Phone,
                AdmissionDate = created.AdmissionDate,
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

        internal static void ValidateStudentFields(
            string? uniqueCode,
            Guid careerId,
            Guid academicPlanId,
            string? studentCarnet,
            string? nationalId,
            string? firstNames,
            string? firstLastName,
            string? secondLastName,
            string? email,
            string? phone)
        {
            var errors = new Dictionary<string, string[]>();

            var trimmedUniqueCode = uniqueCode?.Trim() ?? string.Empty;
            if (trimmedUniqueCode.Length is < 6 or > 20)
            {
                errors["codigo_unico"] = ["Codigo unico (codigo_unico) es requerido y debe tener entre 6 y 20 caracteres."];
            }

            if (careerId == Guid.Empty)
            {
                errors["id_carrera"] = ["Carrera con id valido es requerido."];
            }

            if (academicPlanId == Guid.Empty)
            {
                errors["id_plan_academico"] = ["id de plan academico valido es requerido."];
            }

            if (string.IsNullOrWhiteSpace(studentCarnet) || studentCarnet.Trim().Length > 20)
            {
                errors["carnet"] = ["El carnet de estudiante es requerido y debe tener menos de 20 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(nationalId) && nationalId.Trim().Length > 20)
            {
                errors["cedula"] = ["Cedula es requerida y debe tener menos de 20 caracters."];
            }

            if (string.IsNullOrWhiteSpace(firstNames) || firstNames.Trim().Length > 100)
            {
                errors["nombres"] = ["Nombres son requeridos y deben tener menos de 100 caracteres."];
            }

            if (string.IsNullOrWhiteSpace(firstLastName) || firstLastName.Trim().Length > 60)
            {
                errors["primer_apellido"] = ["Apellidos son requeridos y deben tener menos de 60 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(secondLastName) && secondLastName.Trim().Length > 60)
            {
                errors["segundo_apellido"] = ["Apellidos son requeridos y deben tener menos de 60 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(email) && email.Trim().Length > 150)
            {
                errors["correo"] = ["Email debe tener menos de 150 caracteres."];
            }

            if (!string.IsNullOrWhiteSpace(phone) && phone.Trim().Length > 20)
            {
                errors["telefono"] = ["Numero de telefono debe tener menos de 20 caracteres."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Ocurrieron uno o mas errores de validacion.", errors);
            }
        }
    }
}
