using SupFile.Back.Data.Seeds;

namespace SupFile.Back.Data;

public static class ServiceCollectionExtensions
{
    public static void AddDataRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IFolderRepository, FolderRepository>();
        services.AddTransient<IMediaRepository, MediaRepository>();
        services.AddTransient<IShareRepository, ShareRepository>();
        services.AddTransient<ILinkRepository, LinkRepository>();
    }

    public static void AddSeeders(this IServiceCollection services)
    {
        services.AddTransient<DatabaseSeeder>();

        services.AddTransient<UsersSeeder>();
    }
}
