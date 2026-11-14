using Npgsql;
using Supabase.Gotrue.Exceptions;
using System.Text.Json;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.Exceptions;

namespace unhsjm_asistencia_backend.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorCode, message, details) = MapException(exception);

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Exepcion no controlada al ejecutar {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception, "Domain/Database error ({StatusCode} {ErrorCode}): {Message}", statusCode, errorCode, message);
            }

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = statusCode;

            var payload = new ErrorResponseDto
            {
                Status = statusCode,
                Error = errorCode,
                Message = message,
                Details = details
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }

        private static (int StatusCode, string ErrorCode, string Message, IDictionary<string, string[]>? Details) MapException(Exception exception)
        {
            return exception switch
            {
                ApiException apiEx => (apiEx.StatusCode, apiEx.ErrorCode, apiEx.Message, apiEx.Details),

                PostgresException pgEx when pgEx.SqlState == PostgresErrorCodes.UniqueViolation => (
                    StatusCodes.Status409Conflict,
                    "CONFLICT_ERROR",
                    $"Violacion de campo unico: {pgEx.ConstraintName ?? pgEx.MessageText}",
                    pgEx.ConstraintName is not null
                        ? new Dictionary<string, string[]> { [pgEx.ConstraintName] = [pgEx.Detail ?? pgEx.MessageText] }
                        : null),

                PostgresException pgEx when pgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation => (
                    StatusCodes.Status409Conflict,
                    "CONFLICT_ERROR",
                    $"Violacion de llave foranea: {pgEx.ConstraintName ?? pgEx.MessageText}",
                    pgEx.ConstraintName is not null
                        ? new Dictionary<string, string[]> { [pgEx.ConstraintName] = [pgEx.Detail ?? pgEx.MessageText] }
                        : null),

                PostgresException pgEx when pgEx.SqlState == PostgresErrorCodes.CheckViolation => (
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    $"Validacion de base de dato fallida: {pgEx.ConstraintName ?? pgEx.MessageText}",
                    pgEx.ConstraintName is not null
                        ? new Dictionary<string, string[]> { [pgEx.ConstraintName] = [pgEx.MessageText] }
                        : null),

                PostgresException pgEx when pgEx.SqlState == PostgresErrorCodes.InsufficientPrivilege => (
                    StatusCodes.Status403Forbidden,
                    "FORBIDDEN",
                    "No tienes permisos para realizar este operacion.",
                    null),

                GotrueException gotrueEx => MapGotrueException(gotrueEx),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "Error inesperado al procesar la solicitud.",
                    null)
            };
        }

        private static (int StatusCode, string ErrorCode, string Message, IDictionary<string, string[]>? Details) MapGotrueException(GotrueException ex)
        {
            var rawMessage = ex.Message ?? "Error de autenticacion en Supabase Auth.";
            if (rawMessage.Contains("already registered", StringComparison.OrdinalIgnoreCase)
                || rawMessage.Contains("user_already_exists", StringComparison.OrdinalIgnoreCase))
            {
                return (
                    StatusCodes.Status409Conflict,
                    "CONFLICT_ERROR",
                    "Email ya registrado",
                    new Dictionary<string, string[]> { ["email"] = ["Email en uso."] });
            }

            if (rawMessage.Contains("invalid_credentials", StringComparison.OrdinalIgnoreCase)
                || rawMessage.Contains("Invalid login credentials", StringComparison.OrdinalIgnoreCase))
            {
                return (
                    StatusCodes.Status401Unauthorized,
                    "UNAUTHORIZED",
                    "Email o Contraseña invalida",
                    null);
            }

            return (
                StatusCodes.Status400BadRequest,
                "AUTH_ERROR",
                rawMessage,
                null);
        }
    }


}
