using SupFile.Back.Data.Seeds;

namespace SupFile.Back.Data;

public static class ServiceCollectionExtensions
{
    public static void AddDataRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
    }

    public static void AddSeeders(this IServiceCollection services)
    {
        services.AddTransient<DatabaseSeeder>();

        services.AddTransient<UsersSeeder>();
    }
}
