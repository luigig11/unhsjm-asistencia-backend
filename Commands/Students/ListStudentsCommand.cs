using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.DTOs.Students;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Students
{
    public sealed class ListStudentsCommand : CommandBase<PagedResponseDto<StudentListItemDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private StudentFilterDto _filter = new();

        public ListStudentsCommand(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public ListStudentsCommand WithFilter(StudentFilterDto filter)
        {
            _filter = filter;
            return this;
        }

        public override async Task<PagedResponseDto<StudentListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var (total, items) = await _studentRepository.GetPagedAsync(_filter, cancellationToken);

            return new PagedResponseDto<StudentListItemDto>
            {
                Total = total,
                Items = items
                    .Select(s => new StudentListItemDto
                    {
                        Id = s.Id,
                        UniqueCode = s.UniqueCode,
                        StudentCarnet = s.StudentCarnet,
                        NationalId = s.NationalId,
                        FirstNames = s.FirstNames,
                        FirstLastName = s.FirstLastName,
                        SecondLastName = s.SecondLastName,
                        Email = s.Email,
                        Phone = s.Phone,
                        AdmissionDate = s.AdmissionDate,
                        CareerId = s.CareerId,
                        CareerName = s.CareerName,
                        AcademicPlanId = s.AcademicPlanId,
                        AcademicPlanCode = s.AcademicPlanCode,
                        IsActive = s.IsActive
                    })
                    .ToList()
            };
        }
    }
}
