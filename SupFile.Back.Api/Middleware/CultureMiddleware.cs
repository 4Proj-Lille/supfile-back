using Microsoft.AspNetCore.Components;

namespace SupFile.Back.Api.Middleware;

internal sealed class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings;
    }

    [Inject] public IOptions<AppSettings> _appSettings { get; set; }

    public async Task InvokeAsync(HttpContext context)
    {
        string? culture = null;

        // Try getting culture from authenticated user
        if (context.User.Identity?.IsAuthenticated == true)
        {
            culture = context.User.FindFirst(ClaimTypes.Locality)?.Value;
        }

        // Validate and fallback
        if (culture == null || string.IsNullOrWhiteSpace(culture) || !IsValidCulture(culture))
        {
            culture = _appSettings.Value.SupportedCultures.FirstOrDefault();
        }

        if (culture == null)
        {
            throw new Exception("No culture found");
        }

        // Set the culture in the current context
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        // Call the next middleware in the pipeline
        await _next(context);
    }

    private static bool IsValidCulture(string culture)
    {
        try
        {
            var _ = new CultureInfo(culture);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}
