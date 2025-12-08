using SupFile.Back.Api;
using SupFile.Back.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSettings()
    .AddApiServices()
    .AddErrorHandling()
    .AddApplicationServices()
    .AddDatabase()
    .AddMessageBusses()
    .AddAuthenticationServices()
    .AddCorsPolicy();

var logger = builder.AddSerilog();

var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
ArgumentNullException.ThrowIfNull(appSettings);

LogHelper.LogInformation(logger, "[Program]", "App starting...");

// Build the application
var app = builder.Build();

// app.UseCors("SupChat");
app.UseCors(CorsOptions.PolicyName);

// Apply pending migrations if in development mode
// if (app.Environment.IsDevelopment())
// {
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SupFileContext>();
var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
try
{
    // Apply pending migrations
    await dbContext.Database.MigrateAsync();
    LogHelper.LogInformation(logger, "[Program]", "Migrations applied successfully in development mode.");

    await databaseSeeder.SeedDataAsync(dbContext);
}
catch (Exception ex)
{
    LogHelper.LogError(logger, "[Program]", "An error occurred while applying migrations.", ex);
}
// }

app.UseRouting();

// Configure the static files (wwwroot)
app.UseStaticFiles();

// Adds the swagger endpoint if not in production
if (builder.Configuration.GetValue<bool>("AppSettings:AllowSwagger"))
{
    // this is the way to add a swagger in .Net9 but i couldn't make it work with the custom localization parameters
    // app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            // options.SwaggerEndpoint("/openapi/v1.json", builder.Configuration["AppSettings:Name"] ?? "Api");
            options.SwaggerEndpoint($"/swagger/{appSettings.Version}/swagger.json",
                builder.Configuration["AppSettings:Name"] ?? "Api");
            options.DocExpansion(DocExpansion.None); // Ensures all controllers are collapsed by default
        }
    );
}

// Add the network security policies
// app.UseHttpsRedirection();

// Add the authentication
app.UseAuthentication();
app.UseAuthorization();

// Add the middleware to set the culture
app.UseExceptionHandler();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<CultureMiddleware>();

// Add the controllers
app.MapControllers();
app.MapHealthChecks("/health");
// app.MapHub<ChatHub>("/chatHub");

app.MapDefaultControllerRoute();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


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
