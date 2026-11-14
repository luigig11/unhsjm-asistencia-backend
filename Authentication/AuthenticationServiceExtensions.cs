using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using unhsjm_asistencia_backend.Configuration;
using unhsjm_asistencia_backend.DTOs.Common;
using System.Text.Json;

namespace unhsjm_asistencia_backend.Authentication
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddSupabaseAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SupabaseOptions>(
                configuration.GetSection(SupabaseOptions.SectionName));

            services.AddHttpClient(SupabaseJwksProvider.HttpClientName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddSingleton<SupabaseJwksProvider>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<SupabaseJwksProvider, IOptions<SupabaseOptions>>((options, jwksProvider, supabaseOptionsAccessor) =>
                {
                    var supabaseOptions = supabaseOptionsAccessor.Value;

                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = !supabaseOptions.Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeyResolver = (_, _, kid, _) => jwksProvider.ResolveSigningKeys(kid),
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
                        OnMessageReceived = async context =>
                        {
                            var token = ExtractBearerToken(context.Request.Headers.Authorization);
                            await jwksProvider.EnsureKeysForTokenAsync(token, context.HttpContext.RequestAborted);
                        },
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json; charset=utf-8";

                            var payload = new ErrorResponseDto
                            {
                                Status = StatusCodes.Status401Unauthorized,
                                Error = "UNAUTHORIZED",
                                Message = "Missing, invalid, or expired JWT token."
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
                                Message = "Your role does not have permission in the RBAC/RLS matrix to execute this operation."
                            };

                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        }
                    };
                });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireClaim("user_rol", "ADMIN"))
                .AddPolicy("AdminOrDirector", policy => policy.RequireClaim("user_rol", "ADMIN", "DIRECTOR"))
                .AddPolicy("TeacherOnly", policy => policy.RequireClaim("user_rol", "DOCENTE"));

            return services;
        }

        private static string? ExtractBearerToken(string? authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return null;
            }

            const string prefix = "Bearer ";
            return authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader[prefix.Length..].Trim()
                : null;
        }
    }

}
