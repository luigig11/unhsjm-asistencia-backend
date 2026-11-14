using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using unhsjm_asistencia_backend.Configuration;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Common;

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

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

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
