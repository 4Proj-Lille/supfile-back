using Azure.Storage.Blobs;
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
        // services.AddOptions<FileStorageSettings>()
        //     .BindConfiguration(nameof(FileStorageSettings))
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();

        services.AddOptions<BlobStorageSettings>()
            .BindConfiguration(nameof(BlobStorageSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var blobStorageSettings = configuration.GetSection(nameof(BlobStorageSettings)).Get<BlobStorageSettings>();
        if (blobStorageSettings == null)
        {
            throw new InvalidOperationException($"Failed to bind {nameof(BlobStorageSettings)} from configuration.");
        }

        var blobServiceClient = new BlobServiceClient(blobStorageSettings.ConnectionString);
        services.AddSingleton(blobServiceClient);
        // services.AddSingleton<IStorageProvider, FileStorageProvider>();
        services.AddSingleton<IStorageProvider, BlobStorageProvider>();

        return services;
    }}
