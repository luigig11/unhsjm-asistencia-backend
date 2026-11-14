namespace unhsjm_asistencia_backend.Exceptions
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public IDictionary<string, string[]>? Details { get; }

        protected ApiException(
            int statusCode,
            string errorCode,
            string message,
            IDictionary<string, string[]>? details = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }
    }

    public sealed class ValidationApiException : ApiException
    {
        public ValidationApiException(string message, IDictionary<string, string[]>? details = null)
            : base(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", message, details)
        {
        }
    }

    public sealed class UnauthorizedApiException : ApiException
    {
        public UnauthorizedApiException(string message)
            : base(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", message)
        {
        }
    }

    public sealed class ForbiddenApiException : ApiException
    {
        public ForbiddenApiException(string message)
            : base(StatusCodes.Status403Forbidden, "FORBIDDEN", message)
        {
        }
    }

    public sealed class NotFoundApiException : ApiException
    {
        public NotFoundApiException(string message)
            : base(StatusCodes.Status404NotFound, "NOT_FOUND", message)
        {
        }
    }

    public sealed class ConflictApiException : ApiException
    {
        public ConflictApiException(string message, IDictionary<string, string[]>? details = null)
            : base(StatusCodes.Status409Conflict, "CONFLICT_ERROR", message, details)
        {
        }
    }


}
