using SupFile.Back.Api.ExceptionHandlers;
using SupFile.Back.Api.Settings;
using SupFile.Back.Storage;

namespace SupFile.Back.Api;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddSettings(this WebApplicationBuilder builder)
    {
        builder.AddOptions<AppSettings>();
        builder.AddOptions<SmtpSettings>();
        builder.AddOptions<FrontEndSettings>();
        builder.AddOptions<JwtSettings>();
        builder.AddOptions<AuthProviderSettings>();

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
        builder.Services.AddControllers(opt =>
        {
            opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        }).AddJsonOptions(opts =>
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

        builder.Services.AddStorageProviders(builder.Configuration);

        var smptSettings = builder.Configuration.GetSection(nameof(SmtpSettings)).Get<SmtpSettings>();

        // builder.Services
        //     .AddFluentEmail(
        //         smptSettings.MailFrom,
        //         smptSettings.MailFromDisplayName
        //     );

        builder.Services
            .AddFluentEmail(
                smptSettings.MailFrom,
                smptSettings.MailFromDisplayName
            )
            .AddRazorRenderer()
            .AddSmtpSender(() => new SmtpClient
            {
                Host = smptSettings.Server ?? throw new InvalidOperationException(),
                Port = smptSettings.Port,
                EnableSsl = smptSettings.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smptSettings.Username, smptSettings.Password)
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
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
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

    private static WebApplicationBuilder AddOptions<TSettings>(this WebApplicationBuilder builder)
        where TSettings : class
    {
        builder.Services
            .AddOptions<TSettings>()
            .BindConfiguration(typeof(TSettings).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }
}
