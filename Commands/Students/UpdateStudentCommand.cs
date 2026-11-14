using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Students
{
    public sealed class UpdateStudentCommand : CommandBase<StudentDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IStudentRepository _studentRepository;
        private Guid _studentId;
        private UpdateStudentDto? _request;

        public UpdateStudentCommand(IDapperRlsContext dbContext, IStudentRepository studentRepository)
        {
            _dbContext = dbContext;
            _studentRepository = studentRepository;
        }

        public UpdateStudentCommand WithRequest(Guid id, UpdateStudentDto request)
        {
            _studentId = id;
            _request = request;
            return this;
        }

        public override async Task<StudentDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("REquest paylayload es requerido.");

            CreateStudentCommand.ValidateStudentFields(
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

            var updated = await _studentRepository.UpdateAsync(
                _studentId,
                new StudentEntity
                {
                    Id = _studentId,
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
                cancellationToken)
                ?? throw new NotFoundApiException($"Student with id '{_studentId}' was not found.");

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new StudentDto
            {
                Id = updated.Id,
                UniqueCode = updated.UniqueCode,
                CareerId = updated.CareerId,
                AcademicPlanId = updated.AcademicPlanId,
                StudentCarnet = updated.StudentCarnet,
                NationalId = updated.NationalId,
                FirstNames = updated.FirstNames,
                FirstLastName = updated.FirstLastName,
                SecondLastName = updated.SecondLastName,
                Email = updated.Email,
                Phone = updated.Phone,
                AdmissionDate = updated.AdmissionDate,
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
    }
}
