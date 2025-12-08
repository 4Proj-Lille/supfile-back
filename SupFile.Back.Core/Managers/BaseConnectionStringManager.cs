using System.Collections.Concurrent;

namespace SupFile.Back.Core.Managers;

public class BaseConnectionStringManager
{
    #region Protected Methods

    protected string? RegisterConnectionString(string key, string? value)
    {
        _registeredConnectionStrings.TryAdd(key, value);

        return value;
    }

    #endregion

    #region Private Fields

    public const string Section = "ConnectionStrings";

    private readonly ConcurrentDictionary<string, string?> _registeredConnectionStrings = new();

    #endregion
}
