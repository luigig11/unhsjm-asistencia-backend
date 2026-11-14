namespace unhsjm_asistencia_backend.Data
{
    public interface IDapperRlsContext : IAsyncDisposable
    {
        bool HasActiveTransaction { get; }

        Task BeginTransactionAsync(
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default);

        Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default);

        Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default);

        Task SwitchToAuthenticatedContextAsync(string authUserId, CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default);

        Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default);

        Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default);

        Task<T?> ExecuteScalarAsync<T>(
            string sql,
            object? param = null,
            bool asAnonymous = false,
            string? overrideAuthUserId = null,
            CancellationToken cancellationToken = default);
    }


}
