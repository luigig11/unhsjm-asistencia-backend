using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.TeacherPortal;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.TeacherPortal
{
    public sealed class GetMyCoursesCommand : CommandBase<IReadOnlyList<TeacherCourseDto>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid? _academicInstanceId;

        public GetMyCoursesCommand(
            IAttendanceRepository attendanceRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public GetMyCoursesCommand WithAcademicInstanceId(Guid? academicInstanceId)
        {
            _academicInstanceId = academicInstanceId;
            return this;
        }

        public override async Task<IReadOnlyList<TeacherCourseDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (_academicInstanceId.HasValue)
            {
                Validator.Create()
                    .Field("id_instancia_academica", _academicInstanceId.Value, "instancia academica")
                    .RequiredGuid()
                    .ValidateAndThrow();
            }

            var teacherId = await TeacherIdentityResolver.ResolveAuthenticatedTeacherIdAsync(
                _httpContextAccessor,
                _userRepository,
                cancellationToken);

            var items = await _attendanceRepository.GetTeacherCoursesAsync(
                teacherId,
                _academicInstanceId,
                cancellationToken);

            return items
                .Select(c => new TeacherCourseDto
                {
                    GroupSubjectId = c.GroupSubjectId,
                    AcademicInstanceName = c.AcademicInstanceName,
                    GroupCode = c.GroupCode,
                    SubjectCode = c.SubjectCode,
                    SubjectName = c.SubjectName,
                    TotalEnrolled = c.TotalEnrolled,
                    QuotaLimit = c.QuotaLimit
                })
                .ToList();
        }
    }
}