namespace SupFile.Back.Data.Seeds;

public class DatabaseSeeder
{
    private readonly UsersSeeder _usersSeeder;


    public DatabaseSeeder(UsersSeeder usersSeeder)
    {
        _usersSeeder = usersSeeder;
    }

    public async Task SeedDataAsync(SupFileContext dbContext)
    {
        await _usersSeeder.SeedAsync(dbContext);
    }
}
