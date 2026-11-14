using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Teachers;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Teachers
{
    public sealed class ListTeachersCommand : CommandBase<IReadOnlyList<TeacherListItemDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        private TeacherFilterDto _filter = new();

        public ListTeachersCommand(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public ListTeachersCommand WithFilter(TeacherFilterDto filter)
        {
            _filter = filter;
            return this;
        }

        public override async Task<IReadOnlyList<TeacherListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _teacherRepository.GetAllAsync(_filter, cancellationToken);

            return items
                .Select(t => new TeacherListItemDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = $"{t.FirstName} {t.LastName}".Trim(),
                    NationalId = t.NationalId,
                    DepartmentId = t.DepartmentId,
                    DepartmentName = t.DepartmentName,
                    EmployeeCode = t.EmployeeCode,
                    Phone = t.Phone,
                    ContractType = t.ContractType,
                    IsActive = t.IsActive
                })
                .ToList();
        }
    }
}
