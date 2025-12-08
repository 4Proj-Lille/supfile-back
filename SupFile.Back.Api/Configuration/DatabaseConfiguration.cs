using SupFile.Back.Core.Managers;

namespace SupFile.Back.Api.Configuration;

internal static class DatabaseConfiguration
{
    public static void AddConnectionStrings<TOptions>(this WebApplicationBuilder builder,
        string connectionStringsSectionName = BaseConnectionStringManager.Section)
        where TOptions : BaseConnectionStringManager
    {
        var section = builder.Configuration.GetSection(connectionStringsSectionName);
        builder.Services.Configure<TOptions>(section);

        var connectionStringManager = builder.Services.BuildServiceProvider().GetService<IOptions<TOptions>>();
        if (connectionStringManager != null)
        {
            builder.Services.AddSingleton<IOptions<BaseConnectionStringManager>>(connectionStringManager);
        }
    }
}
