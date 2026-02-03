using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using SupFile.Back.Business.Processors;

namespace SupFile.Back.Business;

public static class ServiceCollectionExtensions
{
    public static void AddBusinessServices(this IServiceCollection services, IConfiguration Configuration)
    {
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();

        services.AddTransient<IAuthTokenProcessor, AuthTokenProcessor>();

        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton(_ => new BlobServiceClient(
            Configuration.GetConnectionString("BlobStorage") ??
            throw new InvalidOperationException("Default connection string missing inside appsettings.json")
        ));
        
        services.AddTransient<IEmailService, EmailService>();

    }
}
