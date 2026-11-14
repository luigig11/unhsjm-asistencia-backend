using unhsjm_asistencia_backend.Authentication;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Commands.Auth;
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
