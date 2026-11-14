using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Teachers
{
    public sealed class GetTeacherByIdCommand : CommandBase<TeacherDetailDto>
    {
        private readonly ITeacherRepository _teacherRepository;
        private Guid _teacherId;

        public GetTeacherByIdCommand(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public GetTeacherByIdCommand WithId(Guid id)
        {
            _teacherId = id;
            return this;
        }

        public override async Task<TeacherDetailDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var teacher = await _teacherRepository.GetByIdWithUserAsync(_teacherId, cancellationToken)
                ?? throw new NotFoundApiException($"Maestro con id '{_teacherId}' no fue encontrado.");

            return new TeacherDetailDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                User = new TeacherUserSummaryDto
                {
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    NationalId = teacher.NationalId,
                    Gender = teacher.Gender
                },
                DepartmentId = teacher.DepartmentId,
                EmployeeCode = teacher.EmployeeCode,
                Phone = teacher.Phone,
                ContractType = teacher.ContractType,
                IsActive = teacher.IsActive
            };
        }
    }
}
