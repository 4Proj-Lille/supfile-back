using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupFile.Back.Storage.Configuration;
using SupFile.Back.Storage.Interfaces;
using SupFile.Back.Storage.Providers;

namespace SupFile.Back.Storage;

/// <summary>
/// Provides extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageProviders(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddOptions<BlobStorageSettings>()
            .BindConfiguration(nameof(BlobStorageSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var blobStorageSettings = configuration.GetSection(nameof(BlobStorageSettings)).Get<BlobStorageSettings>();
        if (blobStorageSettings == null)
        {
            throw new InvalidOperationException($"Failed to bind {nameof(BlobStorageSettings)} from configuration.");
        }

        var options = environment.IsDevelopment()
            ? new BlobClientOptions
            {
                Retry =
                {
                    MaxRetries = 1,
                    Mode = RetryMode.Fixed,
                    Delay = TimeSpan.FromMilliseconds(200),
                    MaxDelay = TimeSpan.FromSeconds(1),
                    NetworkTimeout = TimeSpan.FromSeconds(10)
                }
            }
            : new BlobClientOptions
            {
                Retry =
                {
                    MaxRetries = 3,
                    Mode = RetryMode.Exponential,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(30),
                    NetworkTimeout = TimeSpan.FromSeconds(60)
                }
            };

        services.AddSingleton(_ => new BlobServiceClient(blobStorageSettings.ConnectionString, options));
        // services.AddSingleton<IStorageProvider, FileStorageProvider>();
        services.AddSingleton<IStorageProvider, BlobStorageProvider>();

        return services;
    }
}
