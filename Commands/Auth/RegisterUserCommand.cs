using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Repositories.Interfaces;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;

namespace unhsjm_asistencia_backend.Commands.Auth
{
    public sealed class RegisterUserCommand : CommandBase<RegisterResponseDto>
    {
        private const string ValidationSavepointName = "sp_validate_user";

        private readonly IDapperRlsContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISupabaseClientFactory _supabaseClientFactory;
        private RegisterRequestDto? _request;

        public RegisterUserCommand(
            IDapperRlsContext dbContext,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ISupabaseClientFactory supabaseClientFactory)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _supabaseClientFactory = supabaseClientFactory;
        }

        public RegisterUserCommand WithRequest(RegisterRequestDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<RegisterResponseDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload was not provided to RegisterUserCommand.");
            ValidateRequest(dto);

            var normalizedGender = dto.Gender.Trim().ToUpperInvariant();
            var userEntity = new UserEntity
            {
                AuthUserId = null,
                RoleId = dto.RoleId,
                UniqueCode = dto.UniqueCode,
                NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Gender = normalizedGender,
                BirthDate = dto.BirthDate,
                IsActive = true
            };

            // Paso 1: Iniciar transaccion Dapper con rol 'anon' para validar las restricciones de la tabla public.usuarios. Despues revertir a transaccion al SAVEPOINT
            await _dbContext.BeginTransactionAsync(asAnonymous: true, cancellationToken: cancellationToken);

            var role = await _roleRepository.GetByIdAsync(dto.RoleId, asAnonymous: true, cancellationToken: cancellationToken)
                ?? throw new ValidationApiException(
                    "El rol especificado no existe o esta inactivo",
                    new Dictionary<string, string[]> { ["id_rol"] = ["Rol no encontrado o inactivo"] });

            await _dbContext.CreateSavepointAsync(ValidationSavepointName, cancellationToken);
            await _userRepository.InsertAnonUserAsync(userEntity, cancellationToken);
            await _dbContext.RollbackToSavepointAsync(ValidationSavepointName, cancellationToken);

            // Paso 2: Crear cuenta en supabase auth (auth.users)
            var supabaseClient = await _supabaseClientFactory.CreateClientAsync();
            var signUpSession = await supabaseClient.Auth.SignUp(dto.Email.Trim(), dto.Password);

            if (signUpSession?.User?.Id is null || !Guid.TryParse(signUpSession.User.Id, out var authUserId))
            {
                throw new ValidationApiException("Failed to create the user account in Supabase Auth.");
            }

            //paso 3: Intertar en la tabla public.usuario el usuario recien creado y commit.
            userEntity.AuthUserId = authUserId;
            var createdUserId = await _userRepository.InsertAnonUserAsync(userEntity, cancellationToken);
            await _dbContext.CommitTransactionAsync(cancellationToken);

            // Step 4: Refresh session after COMMIT so custom_access_token_hook injects user_rol and id_usuario
            //paso 4: refrescar la sesion despues del commit para que el hook de BD custom_access_token_hook inyecte los campos user_role y id_usuario a la sesion.
            var finalSession = signUpSession;
            if (!string.IsNullOrWhiteSpace(signUpSession.AccessToken) && !string.IsNullOrWhiteSpace(signUpSession.RefreshToken))
            {
                await supabaseClient.Auth.SetSession(signUpSession.AccessToken, signUpSession.RefreshToken);
                var refreshedSession = await supabaseClient.Auth.RefreshSession();
                if (refreshedSession is not null)
                {
                    finalSession = refreshedSession;
                }
            }

            return new RegisterResponseDto
            {
                AccessToken = finalSession.AccessToken ?? string.Empty,
                RefreshToken = finalSession.RefreshToken ?? string.Empty,
                ExpiresIn = (int)finalSession.ExpiresIn,
                User = new RegisteredUserSummaryDto
                {
                    Id = createdUserId,
                    AuthUserId = authUserId,
                    UniqueCode = userEntity.UniqueCode!,
                    RoleId = dto.RoleId,
                    RoleCode = role.Code,
                    NationalId = userEntity.NationalId,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Gender = userEntity.Gender,
                    IsActive = true
                }
            };
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.HasActiveTransaction)
            {
                await _dbContext.RollbackTransactionAsync(cancellationToken);
            }
        }

        private static void ValidateRequest(RegisterRequestDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                errors["email"] = ["Email is required."];
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            {
                errors["password"] = ["Password is required and must be at least 6 characters long."];
            }

            if (dto.RoleId == Guid.Empty)
            {
                errors["id_rol"] = ["A valid role UUID is required."];
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                errors["nombre"] = ["First name is required."];
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                errors["apellido"] = ["Last name is required."];
            }

            var gender = dto.Gender?.Trim().ToUpperInvariant();
            if (gender is not ("F" or "M"))
            {
                errors["genero"] = ["Gender must be 'F' or 'M'."];
            }

            if (dto.BirthDate.HasValue && dto.BirthDate.Value >= TimeZoneHelper.TodayInManagua())
            {
                errors["fecha_nacimiento"] = ["Birth date must be earlier than the current date."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("One or more validation errors occurred.", errors);
            }
        }
    }
}
