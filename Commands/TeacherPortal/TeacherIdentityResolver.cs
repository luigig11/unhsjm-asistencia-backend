using System.Security.Claims;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.TeacherPortal
{
    internal static class TeacherIdentityResolver
    {
        public static async Task<Guid> ResolveAuthenticatedTeacherIdAsync(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            CancellationToken cancellationToken)
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var subjectClaim = principal?.FindFirst("sub")?.Value
                ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subjectClaim, out var authUserId))
            {
                throw new UnauthorizedApiException("No se encontro usuario.");
            }

            var user = await userRepository.GetByAuthUserIdAsync(authUserId, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new NotFoundApiException("Perfil de usuario no encontrado.");
            }

            if (!user.TeacherId.HasValue || user.TeacherId.Value == Guid.Empty)
            {
                throw new ForbiddenApiException("Usuairo no tiene asignado perfil de maestro.");
            }

            return user.TeacherId.Value;
        }
    }
}