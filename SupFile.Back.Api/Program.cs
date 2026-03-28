using SupFile.Back.Api;
using SupFile.Back.Api.Middlewares;
using SupFile.Back.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSettings()
    .AddSerilog()
    .AddApiServices()
    .AddErrorHandling()
    .AddApplicationServices()
    .AddDatabase()
    .AddMessageBusses()
    .AddAuthenticationServices()
    .AddCorsPolicy();

// Build the application
var app = builder.Build();

var appSettings = app.Services.GetRequiredService<IOptions<AppSettings>>().Value;
var logger = app.Services.GetRequiredService<ILogger<Program>>();
LogHelper.LogInformation(logger, $"[{nameof(Program)}]", "{0} {1} {2} starting...", 
     appSettings.Name, appSettings.Environment, appSettings.Version);

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseCors("SupFile");
app.UseCors(CorsOptions.PolicyName);

// Apply pending migrations if in development mode
// if (app.Environment.IsDevelopment())
// {
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SupFileContext>();
var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
try
{
    LogHelper.LogInformation(logger, "[Program]", "Trying to apply migrations...");

    // Apply pending migrations
    await dbContext.Database.MigrateAsync();
    LogHelper.LogInformation(logger, "[Program]", "Migrations applied successfully in development mode.");

    await databaseSeeder.SeedDataAsync(dbContext);
}
catch (Exception ex)
{
    LogHelper.LogError(logger, "[Program]", ex, "An error occurred while applying migrations.");
}
// }

app.UseRouting();

// Configure the static files (wwwroot)
app.UseStaticFiles();

// Adds the swagger endpoint if not in production
if (appSettings.AllowSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                $"/swagger/{appSettings.Version}/swagger.json",
                appSettings.Name ?? "Api"
            );
            options.DocExpansion(DocExpansion.None);
        }
    );
}

// Add the network security policies
app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

// Add the authentication
app.UseAuthentication();
app.UseAuthorization();

// Add the middleware to set the culture
app.UseExceptionHandler();

// Add the controllers
app.MapControllers();
app.MapHealthChecks("/health");
// app.MapHub<ChatHub>("/chatHub");

app.MapDefaultControllerRoute();



// Log the URLs that are being listened to
app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var address in app.Urls)
    {
        LogHelper.LogInformation(logger, "[Program]", $"App is running on '{address}/swagger'");
    }
});

LogHelper.LogInformation(logger, "[Program]", "App is started");

await app.RunAsync();
