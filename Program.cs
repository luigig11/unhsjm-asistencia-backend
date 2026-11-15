using plantilla.Repositories;
using unhsjm_asistencia_backend.Authentication;
using unhsjm_asistencia_backend.Commands.AbsenceReasons;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicCycles;
using unhsjm_asistencia_backend.Commands.AcademicInstances;
using unhsjm_asistencia_backend.Commands.AcademicPeriods;
using unhsjm_asistencia_backend.Commands.AcademicPeriodTypes;
using unhsjm_asistencia_backend.Commands.AcademicPlanDetails;
using unhsjm_asistencia_backend.Commands.AcademicPlans;
using unhsjm_asistencia_backend.Commands.Attendances;
using unhsjm_asistencia_backend.Commands.Auth;
using unhsjm_asistencia_backend.Commands.CalendarSessions;
using unhsjm_asistencia_backend.Commands.Carrers;
using unhsjm_asistencia_backend.Commands.Classrooms;
using unhsjm_asistencia_backend.Commands.ClassroomTypes;
using unhsjm_asistencia_backend.Commands.CurricularGridItems;
using unhsjm_asistencia_backend.Commands.Days;
using unhsjm_asistencia_backend.Commands.Departments;
using unhsjm_asistencia_backend.Commands.EventTypes;
using unhsjm_asistencia_backend.Commands.GroupEnrollments;
using unhsjm_asistencia_backend.Commands.Groups;
using unhsjm_asistencia_backend.Commands.GroupSubjects;
using unhsjm_asistencia_backend.Commands.Modalities;
using unhsjm_asistencia_backend.Commands.Permissions;
using unhsjm_asistencia_backend.Commands.Roles;
using unhsjm_asistencia_backend.Commands.SchoolPeriods;
using unhsjm_asistencia_backend.Commands.Students;
using unhsjm_asistencia_backend.Commands.SubjectPrerequisites;
using unhsjm_asistencia_backend.Commands.Subjects;
using unhsjm_asistencia_backend.Commands.Teachers;
using unhsjm_asistencia_backend.Commands.UniversityCenters;
using unhsjm_asistencia_backend.Commands.Users;
using unhsjm_asistencia_backend.Commands.WeeklySchedules;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Middleware;
using unhsjm_asistencia_backend.Repositories;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSupabaseAuthentication(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            //Cliente Supabase y Dapper
            builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
            builder.Services.AddScoped<IDapperRlsContext, DapperRlsContext>();

            //Repositorios
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<ICareerRepository, CareerRepository>();
            builder.Services.AddScoped<IModalityRepository, ModalityRepository>();
            builder.Services.AddScoped<IAcademicPeriodTypeRepository, AcademicPeriodTypeRepository>();
            builder.Services.AddScoped<IAcademicPeriodRepository, AcademicPeriodRepository>();
            builder.Services.AddScoped<IAcademicCycleRepository, AcademicCycleRepository>();
            builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
            builder.Services.AddScoped<IUniversityCenterRepository, UniversityCenterRepository>();
            builder.Services.AddScoped<IClassroomTypeRepository, ClassroomTypeRepository>();
            builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
            builder.Services.AddScoped<IDayRepository, DayRepository>();
            builder.Services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            builder.Services.AddScoped<IAbsenceReasonRepository, AbsenceReasonRepository>();
            builder.Services.AddScoped<IAcademicPlanRepository, AcademicPlanRepository>();
            builder.Services.AddScoped<IAcademicPlanDetailRepository, AcademicPlanDetailRepository>();
            builder.Services.AddScoped<ICurricularGridRepository, CurricularGridRepository>();
            builder.Services.AddScoped<ISubjectPrerequisiteRepository, SubjectPrerequisiteRepository>();
            builder.Services.AddScoped<ISchoolPeriodRepository, SchoolPeriodRepository>();
            builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            builder.Services.AddScoped<IAcademicInstanceRepository, AcademicInstanceRepository>();
            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<IGroupSubjectRepository, GroupSubjectRepository>();
            builder.Services.AddScoped<IWeeklyScheduleRepository, WeeklyScheduleRepository>();
            builder.Services.AddScoped<ICalendarSessionRepository, CalendarSessionRepository>();
            builder.Services.AddScoped<IGroupEnrollmentRepository, GroupEnrollmentRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

            //Comandos
            builder.Services.AddScoped<ICommandInvoker, CommandInvoker>();
            builder.Services.AddScoped<RegisterUserCommand>();
            builder.Services.AddScoped<LoginCommand>();
            builder.Services.AddScoped<RefreshTokenCommand>();
            builder.Services.AddScoped<LogoutCommand>();
            builder.Services.AddScoped<GetCurrentProfileCommand>();
            builder.Services.AddScoped<ListUsersCommand>();
            builder.Services.AddScoped<GetUserByIdCommand>();
            builder.Services.AddScoped<UpdateUserCommand>();
            builder.Services.AddScoped<DeleteUserCommand>();
            builder.Services.AddScoped<ListRolesCommand>();
            builder.Services.AddScoped<CreateRoleCommand>();
            builder.Services.AddScoped<UpdateRoleCommand>();
            builder.Services.AddScoped<DeleteRoleCommand>();
            builder.Services.AddScoped<ListPermissionsCommand>();
            builder.Services.AddScoped<CreatePermissionCommand>();
            builder.Services.AddScoped<UpdatePermissionCommand>();
            builder.Services.AddScoped<DeletePermissionCommand>();
            builder.Services.AddScoped<ListTeachersCommand>();
            builder.Services.AddScoped<GetTeacherByIdCommand>();
            builder.Services.AddScoped<CreateTeacherCommand>();
            builder.Services.AddScoped<UpdateTeacherCommand>();
            builder.Services.AddScoped<DeleteTeacherCommand>();
            builder.Services.AddScoped<ListStudentsCommand>();
            builder.Services.AddScoped<GetStudentByIdCommand>();
            builder.Services.AddScoped<CreateStudentCommand>();
            builder.Services.AddScoped<UpdateStudentCommand>();
            builder.Services.AddScoped<DeleteStudentCommand>();
            builder.Services.AddScoped<ListDepartmentsCommand>();
            builder.Services.AddScoped<GetDepartmentByIdCommand>();
            builder.Services.AddScoped<CreateDepartmentCommand>();
            builder.Services.AddScoped<UpdateDepartmentCommand>();
            builder.Services.AddScoped<DeleteDepartmentCommand>();
            builder.Services.AddScoped<ListCareersCommand>();
            builder.Services.AddScoped<GetCareerByIdCommand>();
            builder.Services.AddScoped<CreateCareerCommand>();
            builder.Services.AddScoped<UpdateCareerCommand>();
            builder.Services.AddScoped<DeleteCareerCommand>();
            builder.Services.AddScoped<ListModalitiesCommand>();
            builder.Services.AddScoped<CreateModalityCommand>();
            builder.Services.AddScoped<UpdateModalityCommand>();
            builder.Services.AddScoped<DeleteModalityCommand>();
            builder.Services.AddScoped<ListAcademicPeriodTypesCommand>();
            builder.Services.AddScoped<CreateAcademicPeriodTypeCommand>();
            builder.Services.AddScoped<UpdateAcademicPeriodTypeCommand>();
            builder.Services.AddScoped<DeleteAcademicPeriodTypeCommand>();
            builder.Services.AddScoped<ListAcademicPeriodsCommand>();
            builder.Services.AddScoped<CreateAcademicPeriodCommand>();
            builder.Services.AddScoped<UpdateAcademicPeriodCommand>();
            builder.Services.AddScoped<DeleteAcademicPeriodCommand>();
            builder.Services.AddScoped<ListAcademicCyclesCommand>();
            builder.Services.AddScoped<CreateAcademicCycleCommand>();
            builder.Services.AddScoped<UpdateAcademicCycleCommand>();
            builder.Services.AddScoped<DeleteAcademicCycleCommand>();
            builder.Services.AddScoped<ListSubjectsCommand>();
            builder.Services.AddScoped<CreateSubjectCommand>();
            builder.Services.AddScoped<UpdateSubjectCommand>();
            builder.Services.AddScoped<DeleteSubjectCommand>();
            builder.Services.AddScoped<ListUniversityCentersCommand>();
            builder.Services.AddScoped<CreateUniversityCenterCommand>();
            builder.Services.AddScoped<UpdateUniversityCenterCommand>();
            builder.Services.AddScoped<ListClassroomTypesCommand>();
            builder.Services.AddScoped<CreateClassroomTypeCommand>();
            builder.Services.AddScoped<UpdateClassroomTypeCommand>();
            builder.Services.AddScoped<ListClassroomsCommand>();
            builder.Services.AddScoped<CreateClassroomCommand>();
            builder.Services.AddScoped<UpdateClassroomCommand>();
            builder.Services.AddScoped<ListDaysCommand>();
            builder.Services.AddScoped<ListEventTypesCommand>();
            builder.Services.AddScoped<CreateEventTypeCommand>();
            builder.Services.AddScoped<UpdateEventTypeCommand>();
            builder.Services.AddScoped<ListAbsenceReasonsCommand>();
            builder.Services.AddScoped<CreateAbsenceReasonCommand>();
            builder.Services.AddScoped<UpdateAbsenceReasonCommand>();
            builder.Services.AddScoped<ListAcademicPlansCommand>();
            builder.Services.AddScoped<GetAcademicPlanByIdCommand>();
            builder.Services.AddScoped<GetFullCurriculumGridCommand>();
            builder.Services.AddScoped<CreateAcademicPlanCommand>();
            builder.Services.AddScoped<UpdateAcademicPlanCommand>();
            builder.Services.AddScoped<ListAcademicPlanDetailsCommand>();
            builder.Services.AddScoped<CreateAcademicPlanDetailCommand>();
            builder.Services.AddScoped<UpdateAcademicPlanDetailCommand>();
            builder.Services.AddScoped<CreateCurricularGridItemCommand>();
            builder.Services.AddScoped<UpdateCurricularGridItemCommand>();
            builder.Services.AddScoped<CreateSubjectPrerequisiteCommand>();
            builder.Services.AddScoped<DeleteSubjectPrerequisiteCommand>();
            builder.Services.AddScoped<ListSchoolPeriodsCommand>();
            builder.Services.AddScoped<CreateSchoolPeriodCommand>();
            builder.Services.AddScoped<UpdateSchoolPeriodCommand>();
            builder.Services.AddScoped<ListAcademicInstancesCommand>();
            builder.Services.AddScoped<CreateAcademicInstanceCommand>();
            builder.Services.AddScoped<UpdateAcademicInstanceCommand>();
            builder.Services.AddScoped<ListGroupsCommand>();
            builder.Services.AddScoped<CreateGroupCommand>();
            builder.Services.AddScoped<UpdateGroupCommand>();
            builder.Services.AddScoped<ListGroupSubjectsCommand>();
            builder.Services.AddScoped<CreateGroupSubjectCommand>();
            builder.Services.AddScoped<UpdateGroupSubjectCommand>();
            builder.Services.AddScoped<ListWeeklySchedulesCommand>();
            builder.Services.AddScoped<CreateWeeklyScheduleCommand>();
            builder.Services.AddScoped<UpdateWeeklyScheduleCommand>();
            builder.Services.AddScoped<ListCalendarSessionsCommand>();
            builder.Services.AddScoped<CreateCalendarSessionCommand>();
            builder.Services.AddScoped<UpdateCalendarSessionCommand>();
            builder.Services.AddScoped<GenerateCalendarSessionsCommand>();
            builder.Services.AddScoped<ListGroupEnrollmentsCommand>();
            builder.Services.AddScoped<CreateGroupEnrollmentCommand>();
            builder.Services.AddScoped<UpdateGroupEnrollmentCommand>();
            builder.Services.AddScoped<GetSessionAttendanceSheetCommand>();
            builder.Services.AddScoped<SaveSessionAttendanceSheetCommand>();
            builder.Services.AddScoped<UpdateAttendanceDetailCommand>();
            builder.Services.AddScoped<GetGroupAttendanceRosterCommand>();
            builder.Services.AddScoped<GetDashboardSummaryCommand>();
            builder.Services.AddScoped<GetMyTodaySessionsCommand>();
            builder.Services.AddScoped<GetMyCoursesCommand>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            //app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
