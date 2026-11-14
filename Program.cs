using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Auth;
using unhsjm_asistencia_backend.Configuration;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Common;
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

            builder.Services.Configure<SupabaseOptions>(
                builder.Configuration.GetSection(SupabaseOptions.SectionName));

            var supabaseOptions = builder.Configuration
                .GetSection(SupabaseOptions.SectionName)
                .Get<SupabaseOptions>() ?? new SupabaseOptions();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    var secretBytes = Encoding.UTF8.GetBytes(
                        string.IsNullOrWhiteSpace(supabaseOptions.JwtSecret)
                        ? "default-development-placeholder-jwt-secret-key-32-bytes"
                        : supabaseOptions.JwtSecret);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                        ValidateIssuer = !string.IsNullOrWhiteSpace(supabaseOptions.ValidIssuer),
                        ValidIssuer = supabaseOptions.ValidIssuer,
                        ValidateAudience = !string.IsNullOrWhiteSpace(supabaseOptions.ValidAudience),
                        ValidAudience = supabaseOptions.ValidAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = "sub",
                        RoleClaimType = "user_rol"

                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json; charset=utf-8";

                            var payload = new ErrorResponseDto
                            {
                                Status = StatusCodes.Status401Unauthorized,
                                Error = "UNAUTHORIZED",
                                Message = "Token expirado o invalido"
                            };

                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json; charset=utf-8";

                            var payload = new ErrorResponseDto
                            {
                                Status = StatusCodes.Status403Forbidden,
                                Error = "FORBIDDEN",
                                Message = "Accion no autorizada."
                            };

                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        }
                    };
                });

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireClaim("user_rol", "ADMIN"))
                .AddPolicy("AdminOrDirector", policy => policy.RequireClaim("user_rol", "ADMIN", "DIRECTOR"))
                .AddPolicy("TeacherOnly", policy => policy.RequireClaim("user_rol", "DOCENTE"));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<ISupabaseClientFactory, SupabaseClientFactory>();
            builder.Services.AddScoped<IDapperRlsContext, DapperRlsContext>();

            // Repositories (Module 1, Module 2, Module 3 & Module 4)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            //builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            //builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
            //builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //builder.Services.AddScoped<ICareerRepository, CareerRepository>();
            //builder.Services.AddScoped<IModalityRepository, ModalityRepository>();
            //builder.Services.AddScoped<IAcademicPeriodTypeRepository, AcademicPeriodTypeRepository>();
            //builder.Services.AddScoped<IAcademicPeriodRepository, AcademicPeriodRepository>();
            //builder.Services.AddScoped<IAcademicCycleRepository, AcademicCycleRepository>();
            //builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
            //builder.Services.AddScoped<IUniversityCenterRepository, UniversityCenterRepository>();
            //builder.Services.AddScoped<IClassroomTypeRepository, ClassroomTypeRepository>();
            //builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
            //builder.Services.AddScoped<IDayRepository, DayRepository>();
            //builder.Services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            //builder.Services.AddScoped<IAbsenceReasonRepository, AbsenceReasonRepository>();
            //builder.Services.AddScoped<IAcademicPlanRepository, AcademicPlanRepository>();
            //builder.Services.AddScoped<IAcademicPlanDetailRepository, AcademicPlanDetailRepository>();
            //builder.Services.AddScoped<ICurricularGridRepository, CurricularGridRepository>();
            //builder.Services.AddScoped<ISubjectPrerequisiteRepository, SubjectPrerequisiteRepository>();
            //builder.Services.AddScoped<ISchoolPeriodRepository, SchoolPeriodRepository>();
            //builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            //builder.Services.AddScoped<IAcademicInstanceRepository, AcademicInstanceRepository>();
            //builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            //builder.Services.AddScoped<IGroupSubjectRepository, GroupSubjectRepository>();
            //builder.Services.AddScoped<IWeeklyScheduleRepository, WeeklyScheduleRepository>();
            //builder.Services.AddScoped<ICalendarSessionRepository, CalendarSessionRepository>();
            //builder.Services.AddScoped<IGroupEnrollmentRepository, GroupEnrollmentRepository>();
            //builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

            // Command Invoker
            builder.Services.AddScoped<ICommandInvoker, CommandInvoker>();

            // Module 1: Auth Commands
            builder.Services.AddScoped<RegisterUserCommand>();
            builder.Services.AddScoped<LoginCommand>();
            //builder.Services.AddScoped<RefreshTokenCommand>();
            //builder.Services.AddScoped<LogoutCommand>();
            //builder.Services.AddScoped<GetCurrentProfileCommand>();

            // Module 1: Users Commands
            //builder.Services.AddScoped<ListUsersCommand>();
            //builder.Services.AddScoped<GetUserByIdCommand>();
            //builder.Services.AddScoped<UpdateUserCommand>();
            //builder.Services.AddScoped<DeleteUserCommand>();

            // Module 1: Roles Commands
            //builder.Services.AddScoped<ListRolesCommand>();
            //builder.Services.AddScoped<CreateRoleCommand>();
            //builder.Services.AddScoped<UpdateRoleCommand>();
            //builder.Services.AddScoped<DeleteRoleCommand>();

            // Module 1: Permissions Commands
            //builder.Services.AddScoped<ListPermissionsCommand>();
            //builder.Services.AddScoped<CreatePermissionCommand>();
            //builder.Services.AddScoped<UpdatePermissionCommand>();
            //builder.Services.AddScoped<DeletePermissionCommand>();

            // Module 2: Teachers Commands
            //builder.Services.AddScoped<ListTeachersCommand>();
            //builder.Services.AddScoped<GetTeacherByIdCommand>();
            //builder.Services.AddScoped<CreateTeacherCommand>();
            //builder.Services.AddScoped<UpdateTeacherCommand>();
            //builder.Services.AddScoped<DeleteTeacherCommand>();

            // Module 2: Students Commands
            //builder.Services.AddScoped<ListStudentsCommand>();
            //builder.Services.AddScoped<GetStudentByIdCommand>();
            //builder.Services.AddScoped<CreateStudentCommand>();
            //builder.Services.AddScoped<UpdateStudentCommand>();
            //builder.Services.AddScoped<DeleteStudentCommand>();

            // Module 3: Departments Commands
            //builder.Services.AddScoped<ListDepartmentsCommand>();
            //builder.Services.AddScoped<GetDepartmentByIdCommand>();
            //builder.Services.AddScoped<CreateDepartmentCommand>();
            //builder.Services.AddScoped<UpdateDepartmentCommand>();
            //builder.Services.AddScoped<DeleteDepartmentCommand>();

            // Module 3: Careers Commands
            //builder.Services.AddScoped<ListCareersCommand>();
            //builder.Services.AddScoped<GetCareerByIdCommand>();
            //builder.Services.AddScoped<CreateCareerCommand>();
            //builder.Services.AddScoped<UpdateCareerCommand>();
            //builder.Services.AddScoped<DeleteCareerCommand>();

            // Module 3: Modalities Commands
            //builder.Services.AddScoped<ListModalitiesCommand>();
            //builder.Services.AddScoped<CreateModalityCommand>();
            //builder.Services.AddScoped<UpdateModalityCommand>();
            //builder.Services.AddScoped<DeleteModalityCommand>();

            // Module 3: Academic Period Types Commands
            //builder.Services.AddScoped<ListAcademicPeriodTypesCommand>();
            //builder.Services.AddScoped<CreateAcademicPeriodTypeCommand>();
            //builder.Services.AddScoped<UpdateAcademicPeriodTypeCommand>();
            //builder.Services.AddScoped<DeleteAcademicPeriodTypeCommand>();

            // Module 3: Academic Periods Commands
            //builder.Services.AddScoped<ListAcademicPeriodsCommand>();
            //builder.Services.AddScoped<CreateAcademicPeriodCommand>();
            //builder.Services.AddScoped<UpdateAcademicPeriodCommand>();
            //builder.Services.AddScoped<DeleteAcademicPeriodCommand>();

            // Module 3: Academic Cycles Commands
            //builder.Services.AddScoped<ListAcademicCyclesCommand>();
            //builder.Services.AddScoped<CreateAcademicCycleCommand>();
            //builder.Services.AddScoped<UpdateAcademicCycleCommand>();
            //builder.Services.AddScoped<DeleteAcademicCycleCommand>();

            // Module 3: Subjects Commands
            //builder.Services.AddScoped<ListSubjectsCommand>();
            //builder.Services.AddScoped<CreateSubjectCommand>();
            //builder.Services.AddScoped<UpdateSubjectCommand>();
            //builder.Services.AddScoped<DeleteSubjectCommand>();

            // Module 3: University Centers Commands
            //builder.Services.AddScoped<ListUniversityCentersCommand>();
            //builder.Services.AddScoped<CreateUniversityCenterCommand>();
            //builder.Services.AddScoped<UpdateUniversityCenterCommand>();
            //builder.Services.AddScoped<DeleteUniversityCenterCommand>();

            // Module 3: Classroom Types Commands
            //builder.Services.AddScoped<ListClassroomTypesCommand>();
            //builder.Services.AddScoped<CreateClassroomTypeCommand>();
            //builder.Services.AddScoped<UpdateClassroomTypeCommand>();
            //builder.Services.AddScoped<DeleteClassroomTypeCommand>();

            // Module 3: Classrooms Commands
            //builder.Services.AddScoped<ListClassroomsCommand>();
            //builder.Services.AddScoped<CreateClassroomCommand>();
            //builder.Services.AddScoped<UpdateClassroomCommand>();
            //builder.Services.AddScoped<DeleteClassroomCommand>();

            // Module 3: Days Command
            //builder.Services.AddScoped<ListDaysCommand>();

            // Module 3: Event Types Commands
            //builder.Services.AddScoped<ListEventTypesCommand>();
            //builder.Services.AddScoped<CreateEventTypeCommand>();
            //builder.Services.AddScoped<UpdateEventTypeCommand>();
            //builder.Services.AddScoped<DeleteEventTypeCommand>();

            // Module 3: Absence Reasons Commands
            //builder.Services.AddScoped<ListAbsenceReasonsCommand>();
            //builder.Services.AddScoped<CreateAbsenceReasonCommand>();
            //builder.Services.AddScoped<UpdateAbsenceReasonCommand>();
            //builder.Services.AddScoped<DeleteAbsenceReasonCommand>();

            // Module 4: Academic Plans Commands
            //builder.Services.AddScoped<ListAcademicPlansCommand>();
            //builder.Services.AddScoped<GetAcademicPlanByIdCommand>();
            //builder.Services.AddScoped<GetFullCurriculumGridCommand>();
            //builder.Services.AddScoped<CreateAcademicPlanCommand>();
            //builder.Services.AddScoped<UpdateAcademicPlanCommand>();
            //builder.Services.AddScoped<DeleteAcademicPlanCommand>();

            //// Module 4: Academic Plan Details Commands
            //builder.Services.AddScoped<ListAcademicPlanDetailsCommand>();
            //builder.Services.AddScoped<CreateAcademicPlanDetailCommand>();
            //builder.Services.AddScoped<UpdateAcademicPlanDetailCommand>();
            //builder.Services.AddScoped<DeleteAcademicPlanDetailCommand>();

            //// Module 4: Curricular Grid Items Commands
            //builder.Services.AddScoped<CreateCurricularGridItemCommand>();
            //builder.Services.AddScoped<UpdateCurricularGridItemCommand>();
            //builder.Services.AddScoped<DeleteCurricularGridItemCommand>();

            //// Module 4: Subject Prerequisites Commands
            //builder.Services.AddScoped<CreateSubjectPrerequisiteCommand>();
            //builder.Services.AddScoped<DeleteSubjectPrerequisiteCommand>();

            //// Module 5: School Periods Commands
            //builder.Services.AddScoped<ListSchoolPeriodsCommand>();
            //builder.Services.AddScoped<CreateSchoolPeriodCommand>();
            //builder.Services.AddScoped<UpdateSchoolPeriodCommand>();
            //builder.Services.AddScoped<DeleteSchoolPeriodCommand>();

            //// Module 5: Holidays Commands
            //builder.Services.AddScoped<ListHolidaysCommand>();
            //builder.Services.AddScoped<CreateHolidayCommand>();
            //builder.Services.AddScoped<UpdateHolidayCommand>();
            //builder.Services.AddScoped<DeleteHolidayCommand>();

            //// Module 5: Academic Instances Commands
            //builder.Services.AddScoped<ListAcademicInstancesCommand>();
            //builder.Services.AddScoped<CreateAcademicInstanceCommand>();
            //builder.Services.AddScoped<UpdateAcademicInstanceCommand>();
            //builder.Services.AddScoped<DeleteAcademicInstanceCommand>();

            //// Module 5: Groups Commands
            //builder.Services.AddScoped<ListGroupsCommand>();
            //builder.Services.AddScoped<CreateGroupCommand>();
            //builder.Services.AddScoped<UpdateGroupCommand>();
            //builder.Services.AddScoped<DeleteGroupCommand>();

            //// Module 5: Group Subjects Commands
            //builder.Services.AddScoped<ListGroupSubjectsCommand>();
            //builder.Services.AddScoped<CreateGroupSubjectCommand>();
            //builder.Services.AddScoped<UpdateGroupSubjectCommand>();
            //builder.Services.AddScoped<DeleteGroupSubjectCommand>();

            //// Module 5: Weekly Schedules Commands
            //builder.Services.AddScoped<ListWeeklySchedulesCommand>();
            //builder.Services.AddScoped<CreateWeeklyScheduleCommand>();
            //builder.Services.AddScoped<UpdateWeeklyScheduleCommand>();
            //builder.Services.AddScoped<DeleteWeeklyScheduleCommand>();

            //// Module 5: Calendar Sessions Commands
            //builder.Services.AddScoped<ListCalendarSessionsCommand>();
            //builder.Services.AddScoped<CreateCalendarSessionCommand>();
            //builder.Services.AddScoped<UpdateCalendarSessionCommand>();
            //builder.Services.AddScoped<DeleteCalendarSessionCommand>();
            //builder.Services.AddScoped<GenerateCalendarSessionsCommand>();

            //// Module 5: Group Enrollments Commands
            //builder.Services.AddScoped<ListGroupEnrollmentsCommand>();
            //builder.Services.AddScoped<CreateGroupEnrollmentCommand>();
            //builder.Services.AddScoped<UpdateGroupEnrollmentCommand>();
            //builder.Services.AddScoped<DeleteGroupEnrollmentCommand>();

            //// Module 6: Dashboard, Teacher Portal & Attendance Commands
            //builder.Services.AddScoped<GetDashboardSummaryCommand>();
            //builder.Services.AddScoped<GetMyTodaySessionsCommand>();
            //builder.Services.AddScoped<GetMyCoursesCommand>();
            //builder.Services.AddScoped<GetSessionAttendanceSheetCommand>();
            //builder.Services.AddScoped<SaveSessionAttendanceSheetCommand>();
            //builder.Services.AddScoped<UpdateAttendanceDetailCommand>();
            //builder.Services.AddScoped<DeleteAttendanceCommand>();
            //builder.Services.AddScoped<GetGroupAttendanceRosterCommand>();

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

            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
