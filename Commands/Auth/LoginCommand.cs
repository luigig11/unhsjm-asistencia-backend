using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Auth
{
    public sealed class LoginCommand : CommandBase<LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISupabaseClientFactory _supabaseClientFactory;
        private LoginRequestDto? _request;
        private Supabase.Client? _supabaseClient;

        public LoginCommand(IUserRepository userRepository, ISupabaseClientFactory supabaseClientFactory)
        {
            _userRepository = userRepository;
            _supabaseClientFactory = supabaseClientFactory;
        }

        public LoginCommand WithRequest(LoginRequestDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<LoginResponseDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request Payload incorrecto");

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ValidationApiException(
                    "Faltan credenciales",
                    new Dictionary<string, string[]>
                    {
                        ["credenciales"] = ["Correo y contraseña deben ser enviados"]
                    });
            }

            _supabaseClient = await _supabaseClientFactory.CreateClientAsync();
            var session = await _supabaseClient.Auth.SignIn(dto.Email.Trim(), dto.Password);

            if (session?.User?.Id is null || !Guid.TryParse(session.User.Id, out var authUserId))
            {
                throw new UnauthorizedApiException("Correo o contraseña invalidos.");
            }

            var userProfile = await _userRepository.GetByAuthUserIdAsync(
                authUserId,
                asAnonymous: false,
                cancellationToken: cancellationToken)
                ?? throw new ForbiddenApiException("El usuario no tiene perfil activo.");

            return new LoginResponseDto
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                ExpiresIn = (int)session.ExpiresIn,
                User = new LoggedInUserSummaryDto
                {
                    Id = userProfile.Id,
                    AuthUserId = userProfile.AuthUserId,
                    RoleId = userProfile.RoleId,
                    RoleCode = userProfile.RoleCode,
                    TeacherId = userProfile.TeacherId,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName
                }
            };
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_supabaseClient?.Auth.CurrentSession is not null)
            {
                try
                {
                    await _supabaseClient.Auth.SignOut();
                }
                catch
                {
                    // Ignore sign-out cleanup errors
                }
            }
        }
    }


}
