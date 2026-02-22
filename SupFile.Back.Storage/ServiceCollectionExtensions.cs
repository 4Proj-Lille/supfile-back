using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupFile.Back.Storage.Configuration;
using SupFile.Back.Storage.Interfaces;
using SupFile.Back.Storage.Providers;

namespace SupFile.Back.Storage;

/// <summary>
/// Provides extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageProviders(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(StorageSettings)).Get<StorageSettings>();
        if (settings == null)
            throw new InvalidOperationException("StorageProviders settings are not configured properly.");

        services.AddSingleton(settings);

        services.AddSingleton<IStorageProvider>(sp => new FileStorageProvider(settings));
        // services.AddSingleton<IStorageProvider>(sp => new BlobStorageProvider(settings));

        return services;
    }
}
