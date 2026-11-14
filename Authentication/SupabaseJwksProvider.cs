using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using unhsjm_asistencia_backend.Configuration;

namespace unhsjm_asistencia_backend.Authentication
{
    public sealed class SupabaseJwksProvider
    {
        public const string HttpClientName = "SupabaseJwks";

        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
        private static readonly TimeSpan UnknownKidRefreshCooldown = TimeSpan.FromSeconds(30);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SupabaseJwksProvider> _logger;
        private readonly string _jwksUrl;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        private JsonWebKeySet? _cachedKeySet;
        private DateTimeOffset _lastSuccessfulRefresh = DateTimeOffset.MinValue;
        private DateTimeOffset _lastRefreshAttempt = DateTimeOffset.MinValue;

        public SupabaseJwksProvider(
            IHttpClientFactory httpClientFactory,
            IOptions<SupabaseOptions> supabaseOptions,
            ILogger<SupabaseJwksProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            var url = supabaseOptions.Value.Url;
            _jwksUrl = string.IsNullOrWhiteSpace(url)
                ? string.Empty
                : $"{url.TrimEnd('/')}/auth/v1/.well-known/jwks.json";
        }

        public async Task EnsureKeysForTokenAsync(string? rawToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(_jwksUrl))
            {
                return;
            }

            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(rawToken))
            {
                return;
            }

            var jwt = handler.ReadJsonWebToken(rawToken);
            if (!NeedsRefresh(jwt.Kid))
            {
                return;
            }

            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                if (!NeedsRefresh(jwt.Kid))
                {
                    return;
                }

                _lastRefreshAttempt = DateTimeOffset.UtcNow;
                var client = _httpClientFactory.CreateClient(HttpClientName);
                var jwksJson = await client.GetStringAsync(_jwksUrl, cancellationToken);
                var keySet = new JsonWebKeySet(jwksJson);

                _cachedKeySet = keySet;
                _lastSuccessfulRefresh = DateTimeOffset.UtcNow;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Unable to fetch Supabase JWKS from {JwksUrl}.", _jwksUrl);
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public IEnumerable<SecurityKey> ResolveSigningKeys(string? kid)
        {
            var keySet = _cachedKeySet;
            if (keySet is null)
            {
                return [];
            }

            var keys = keySet.GetSigningKeys();
            if (string.IsNullOrWhiteSpace(kid))
            {
                return keys;
            }

            return keys.Where(k => string.Equals(k.KeyId, kid, StringComparison.Ordinal)).ToList();
        }

        private bool NeedsRefresh(string? kid)
        {
            var now = DateTimeOffset.UtcNow;
            var keySet = _cachedKeySet;

            if (keySet is null || now - _lastSuccessfulRefresh >= CacheDuration)
            {
                return now - _lastRefreshAttempt >= UnknownKidRefreshCooldown || keySet is null;
            }

            if (!string.IsNullOrWhiteSpace(kid)
                && !keySet.GetSigningKeys().Any(k => string.Equals(k.KeyId, kid, StringComparison.Ordinal)))
            {
                return now - _lastRefreshAttempt >= UnknownKidRefreshCooldown;
            }

            return false;
        }
    }

}
