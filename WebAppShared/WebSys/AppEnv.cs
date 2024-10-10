using WebAppShared.Types;

namespace WebAppShared.WebSys;

public static class AppEnv
{
    /// <summary>
    ///     Is Development is not always true like when applying migrations.
    /// </summary>
    public static readonly SetOnce<bool> IsDevelopment = new();

    /// <summary>
    ///     Is Production is only set in production.
    /// </summary>
    public static readonly SetOnce<bool> IsProduction = new();
}