using plantilla.Repositories;
using unhsjm_asistencia_backend.Authentication;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.AcademicCycles;
using unhsjm_asistencia_backend.Commands.AcademicPeriods;
using unhsjm_asistencia_backend.Commands.AcademicPeriodTypes;
using unhsjm_asistencia_backend.Commands.Auth;
using unhsjm_asistencia_backend.Commands.Carrers;
using unhsjm_asistencia_backend.Commands.Departments;
using unhsjm_asistencia_backend.Commands.Modalities;
using unhsjm_asistencia_backend.Commands.Permissions;
using unhsjm_asistencia_backend.Commands.Roles;
using unhsjm_asistencia_backend.Commands.Students;
using unhsjm_asistencia_backend.Commands.Teachers;
using unhsjm_asistencia_backend.Commands.Users;
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
