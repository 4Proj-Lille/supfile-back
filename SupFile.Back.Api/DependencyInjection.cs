using SupFile.Back.Api.Settings;

namespace SupFile.Back.Api;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection(nameof(BlobStorageSettings)));

        return builder;
    }

    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
        ArgumentNullException.ThrowIfNull(appSettings);

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.Secure = CookieSecurePolicy.Always;
        });

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddControllers().AddJsonOptions(opts =>
        {
            var enumConverter = new JsonStringEnumConverter();
            opts.JsonSerializerOptions.Converters.Add(enumConverter);
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.ConfigureLocalization(appSettings);

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddOpenApi();
        builder.Services.ConfigureSwagger(appSettings);

        builder.Services.AddControllersWithViews();

        builder.Services.AddHealthChecks();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddBusinessServices(builder.Configuration);
        builder.Services.AddDataRepositories();
        builder.Services.AddSeeders();

        builder.Services
            .AddFluentEmail(
                builder.Configuration["Email:SenderEmail"],
                builder.Configuration["Email:SenderName"]
            );

        builder.Services
            .AddFluentEmail(
                builder.Configuration["Email:SenderEmail"],
                builder.Configuration["Email:SenderName"]
            )
            .AddRazorRenderer()
            .AddSmtpSender(() => new SmtpClient
            {
                Host = builder.Configuration["Email:Host"] ?? throw new InvalidOperationException(),
                Port = builder.Configuration.GetValue<int>("Email:Port"),
                EnableSsl = builder.Configuration.GetValue<bool>("Email:UseSSl"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    builder.Configuration["Email:Username"],
                    builder.Configuration["Email:Password"]
                )
            });

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();


        builder.Services.AddMapster();


        return builder;
    }

    public static WebApplicationBuilder AddMessageBusses(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.AddConnectionStrings<ConnectionStringManager>();

        var connectionString = builder.Configuration.GetConnectionString("Postgres") ??
                               throw new InvalidOperationException(
                                   "Default connection string missing inside appsettings.json");
        builder.Services.AddDbContextFactory<SupFileContext>(options =>
            {
                options.UseNpgsql(connectionString);

                // Enable detailed error messages and sensitive data logging.
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            },
            ServiceLifetime.Transient
        );

        builder.Services.AddDbContextFactory<SupFileContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // options.UseSqlServer(connectionString);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            },
            ServiceLifetime.Transient
        );
        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<SupFileContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.User.RequireUniqueEmail = true;
        });

        builder.AddAuthentication();


        return builder;
    }

    public static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder)
    {
        var corsOptions = builder.Configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()!;

        builder.Services.AddCors(options =>
        {
        options.AddPolicy(CorsOptions.PolicyName, policy =>
        {
           policy
                .WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
        });

        return builder;
    }
}
