using SupFile.Back.Core.Managers;

namespace SupFile.Back.Api.Configuration;

internal abstract class ConnectionStringManager : BaseConnectionStringManager
{
    private string? _myConnectionString;

    public string? MyConnectionString
    {
        get => _myConnectionString;
        set => _myConnectionString = RegisterConnectionString(nameof(MyConnectionString), value);
    }
}
