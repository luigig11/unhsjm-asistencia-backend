using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Students
{
    public sealed class GetStudentByIdCommand : CommandBase<StudentDto>
    {
        private readonly IStudentRepository _studentRepository;
        private Guid _studentId;

        public GetStudentByIdCommand(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public GetStudentByIdCommand WithId(Guid id)
        {
            _studentId = id;
            return this;
        }

        public override async Task<StudentDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(_studentId, cancellationToken)
                ?? throw new NotFoundApiException($"Estudiante con id '{_studentId}' no fue encontrado.");

            return new StudentDto
            {
                Id = student.Id,
                UniqueCode = student.UniqueCode,
                CareerId = student.CareerId,
                AcademicPlanId = student.AcademicPlanId,
                StudentCarnet = student.StudentCarnet,
                NationalId = student.NationalId,
                FirstNames = student.FirstNames,
                FirstLastName = student.FirstLastName,
                SecondLastName = student.SecondLastName,
                Email = student.Email,
                Phone = student.Phone,
                AdmissionDate = student.AdmissionDate,
                IsActive = student.IsActive
            };
        }
    }
}
