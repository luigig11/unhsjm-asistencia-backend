using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Exceptions;

namespace unhsjm_asistencia_backend.Commands.Auth
{
    public sealed class RefreshTokenCommand : CommandBase<RefreshTokenResponseDto>
    {
        private readonly ISupabaseClientFactory _supabaseClientFactory;
        private RefreshTokenRequestDto? _request;

        public RefreshTokenCommand(ISupabaseClientFactory supabaseClientFactory)
        {
            _supabaseClientFactory = supabaseClientFactory;
        }

        public RefreshTokenCommand WithRequest(RefreshTokenRequestDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<RefreshTokenResponseDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request Payload incorrecto.");

            if (string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                throw new ValidationApiException(
                    "campos access_token y refresh_token son requeridos.",
                    new Dictionary<string, string[]>
                    {
                        ["refresh_token"] = ["access_token y refresh_token deben ser incluidos en request."]
                    });
            }

            var supabaseClient = await _supabaseClientFactory.CreateClientAsync();
            await supabaseClient.Auth.SetSession(dto.AccessToken.Trim(), dto.RefreshToken.Trim());
            var refreshedSession = await supabaseClient.Auth.RefreshSession();

            if (refreshedSession is null || string.IsNullOrWhiteSpace(refreshedSession.AccessToken))
            {
                throw new UnauthorizedApiException("No se pudo refrescar la sesion con el token del request.");
            }

            return new RefreshTokenResponseDto
            {
                AccessToken = refreshedSession.AccessToken,
                RefreshToken = refreshedSession.RefreshToken ?? string.Empty,
                ExpiresIn = (int)refreshedSession.ExpiresIn
            };
        }
    }


}
