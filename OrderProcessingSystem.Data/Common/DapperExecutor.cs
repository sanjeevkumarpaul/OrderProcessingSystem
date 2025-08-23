using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessingSystem.Data.Common;

public static class DapperExecutor
{
    public static async Task<List<T>> QueryAsync<T>(AppDbContext db, string sql, object? param = null, CancellationToken ct = default)
    {
        using var conn = db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);
        var result = await conn.QueryAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
        return result.AsList();
    }

    public static async Task<T?> QuerySingleOrDefaultAsync<T>(AppDbContext db, string sql, object? param = null, CancellationToken ct = default)
    {
        using var conn = db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
    }

    public static async Task<int> ExecuteAsync(AppDbContext db, string sql, object? param = null, CancellationToken ct = default)
    {
        using var conn = db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);
        return await conn.ExecuteAsync(new CommandDefinition(sql, param, cancellationToken: ct));
    }

    public static async Task<long> ExecuteScalarLongAsync(AppDbContext db, string sql, object? param = null, CancellationToken ct = default)
    {
        using var conn = db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);
        var scalar = await conn.ExecuteScalarAsync<long>(new CommandDefinition(sql, param, cancellationToken: ct));
        return scalar;
    }
}
