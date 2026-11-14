using Dapper;
using Npgsql;
using System.Security.Claims;
using System.Text.Json;
using unhsjm_asistencia_backend.Exceptions;

namespace unhsjm_asistencia_backend.Data
{
    public sealed class DapperRlsContext : IDapperRlsContext
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private NpgsqlConnection? _activeConnection;
        private NpgsqlTransaction? _activeTransaction;

        public DapperRlsContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("SupabaseDb")
                ?? throw new InvalidOperationException("Connection string 'SupabaseDb' is not configured.");
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasActiveTransaction => _activeTransaction is not null;

        public async Task BeginTransactionAsync(
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default)
        {
            if (_activeTransaction is not null)
            {
                return;
            }

            _activeConnection = new NpgsqlConnection(_connectionString);
            await _activeConnection.OpenAsync(cancellationToken);
            _activeTransaction = await _activeConnection.BeginTransactionAsync(cancellationToken);

            await ConfigureSessionRlsAsync(_activeConnection, _activeTransaction, asAnonymous, overrideAuthUserId, cancellationToken);
        }

        public async Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default)
        {
            if (_activeTransaction is null)
            {
                throw new InvalidOperationException("No se puede crear un SavePoint sin una transaccio activa");
            }

            await _activeTransaction.SaveAsync(savepointName, cancellationToken);
        }

        public async Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default)
        {
            if (_activeTransaction is null)
            {
                throw new InvalidOperationException("No se puede hacer rollback a un savepoint sin una transaccion activa");
            }

            await _activeTransaction.RollbackAsync(savepointName, cancellationToken);
        }

        public async Task SwitchToAuthenticatedContextAsync(string authUserId, CancellationToken cancellationToken = default)
        {
            if (_activeConnection is null || _activeTransaction is null)
            {
                throw new InvalidOperationException("No se puede cambiar a configuracion RLS sin una transaccion activa");
            }

            await ConfigureSessionRlsAsync(_activeConnection, _activeTransaction, asAnonymous: false, overrideAuthUserId: authUserId, cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_activeTransaction is null)
            {
                return;
            }

            try
            {
                await _activeTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await DisposeActiveTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_activeTransaction is null)
            {
                return;
            }

            try
            {
                await _activeTransaction.RollbackAsync(cancellationToken);
            }
            catch
            {
                // ignorar errores al momento de hacer rollback
            }
            finally
            {
                await DisposeActiveTransactionAsync();
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithinRlsScopeAsync(
                async (conn, tx) =>
                {
                    var cmd = new CommandDefinition(sql, param, tx, cancellationToken: cancellationToken);
                    return await conn.QueryAsync<T>(cmd);
                },
                asAnonymous,
                overrideAuthUserId,
                cancellationToken);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithinRlsScopeAsync(
                async (conn, tx) =>
                {
                    var cmd = new CommandDefinition(sql, param, tx, cancellationToken: cancellationToken);
                    return await conn.QueryFirstOrDefaultAsync<T>(cmd);
                },
                asAnonymous,
                overrideAuthUserId,
                cancellationToken);
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithinRlsScopeAsync(
                async (conn, tx) =>
                {
                    var cmd = new CommandDefinition(sql, param, tx, cancellationToken: cancellationToken);
                    return await conn.ExecuteAsync(cmd);
                },
                asAnonymous,
                overrideAuthUserId,
                cancellationToken);
        }

        public async Task<T?> ExecuteScalarAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithinRlsScopeAsync(
                async (conn, tx) =>
                {
                    var cmd = new CommandDefinition(sql, param, tx, cancellationToken: cancellationToken);
                    return await conn.ExecuteScalarAsync<T>(cmd);
                },
                asAnonymous,
                overrideAuthUserId,
                cancellationToken);
        }

        private async Task<TResult> ExecuteWithinRlsScopeAsync<TResult>(
            Func<NpgsqlConnection, NpgsqlTransaction, Task<TResult>> operation,
            bool asAnonymous,
            string? overrideAuthUserId,
            CancellationToken cancellationToken)
        {
            if (_activeConnection is not null && _activeTransaction is not null)
            {
                return await operation(_activeConnection, _activeTransaction);
            }

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await ConfigureSessionRlsAsync(connection, transaction, asAnonymous, overrideAuthUserId, cancellationToken);

            var result = await operation(connection, transaction);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }

        private async Task ConfigureSessionRlsAsync(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
            bool asAnonymous,
            string? overrideAuthUserId,
            CancellationToken cancellationToken)
        {
            if (asAnonymous)
            {
                const string anonSql = """
                SET LOCAL ROLE anon;
                SELECT set_config('request.jwt.claim.role', 'anon', true),
                       set_config('request.jwt.claims', '{"role":"anon"}', true);
                """;
                var anonCmd = new CommandDefinition(anonSql, transaction: transaction, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(anonCmd);
                return;
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var sub = overrideAuthUserId
                ?? user?.FindFirstValue("sub")
                ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sub))
            {
                throw new UnauthorizedApiException("Campo ('sub') del usuario no se encuentra.");
            }

            var email = user?.FindFirstValue("email") ?? user?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var userRole = user?.FindFirstValue("user_rol") ?? string.Empty;

            var claimsPayload = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["sub"] = sub,
                ["role"] = "authenticated",
                ["email"] = email,
                ["user_rol"] = userRole
            });

            const string authSql = """
            SET LOCAL ROLE authenticated;
            SELECT set_config('request.jwt.claim.sub', @UserId, true),
                   set_config('request.jwt.claim.role', 'authenticated', true),
                   set_config('request.jwt.claims', @ClaimsJson, true);
            """;

            var cmd = new CommandDefinition(
                authSql,
                new { UserId = sub, ClaimsJson = claimsPayload },
                transaction: transaction,
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(cmd);
        }

        private async Task DisposeActiveTransactionAsync()
        {
            if (_activeTransaction is not null)
            {
                await _activeTransaction.DisposeAsync();
                _activeTransaction = null;
            }

            if (_activeConnection is not null)
            {
                await _activeConnection.DisposeAsync();
                _activeConnection = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_activeTransaction is not null)
            {
                await RollbackTransactionAsync();
            }
        }
    }


}
