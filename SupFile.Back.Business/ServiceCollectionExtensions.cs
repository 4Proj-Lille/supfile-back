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
        services.AddTransient<IFolderService, FolderService>();
        services.AddTransient<IMediaService, MediaService>();
        services.AddTransient<IShareService, ShareService>();
        services.AddTransient<ILinkService, LinkService>();
        services.AddTransient<IBinService, BinService>();
         
        
        services.AddTransient<IAuthTokenProcessor, AuthTokenProcessor>();

        services.AddSingleton<IBlobService, BlobService>();
        
        services.AddTransient<IEmailService, EmailService>();

    }
}
