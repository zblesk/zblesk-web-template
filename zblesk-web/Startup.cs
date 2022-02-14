using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using VueCliMiddleware;
using zblesk_web.Configs;
using zblesk_web.Hubs;
using zblesk_web.Models;
using zblesk_web.Services;
using zblesk.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace zblesk_web;

public class Startup
{
    private static Config _config;

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, sectionName: "Logging")
            .CreateLogger();
    }

    public void ConfigureServices(
        IServiceCollection services)
    {
        Directory.CreateDirectory("Data");

        services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        services.AddSpaStaticFiles(configuration => { configuration.RootPath = Path.Combine("ClientApp", "dist"); });


        _config = new Config();
        Configuration.Bind(_config);
        ConfigureDI(services, _config);
        ConfigureLocalization(services);

        Trace.Assert(
            Config.SupportedLanguages.Contains(_config.DefaultCulture),
            "Unsupported Default Culture");

        services.AddSignalR()
            .AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
            });

        ConfigureIdentity(services, _config);
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime lifetime,
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole> roleManager)
    {
        Log.Information("Updating database");
        dbContext.Database.Migrate();
        Log.Information("Setting up for environment {env}", env.EnvironmentName);
        EnsureRoles(roleManager).Wait();
        app.UseRequestLocalization();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseCors(builder =>
        {
            builder.WithOrigins(_config.BaseUrl, _config.Urls)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseSpaStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = PushSerilogProperties;
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<EventHub>("/hubs/events");
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp/";
            if (env.IsDevelopment())
            {
                spa.UseVueCli(npmScript: "serve");
            }
        });

        lifetime.ApplicationStarted.Register(OnAppStarted);
        lifetime.ApplicationStopping.Register(OnAppStopping);
        lifetime.ApplicationStopped.Register(OnAppStopped);
    }

    private static void ConfigureIdentity(
        IServiceCollection services,
        Config config)
    {
        services.AddAuthentication(cfg =>
        {
            cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = config.SecurityKey,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Query.ContainsKey("access_token"))
                        {
                            ctx.Token = ctx.Request.Query["access_token"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = config.DefaultPasswordMinLength;
            options.User.AllowedUserNameCharacters = ""; // all
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        services.AddSingleton<IAuthorizationHandler, OwnerOrAdminAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
            options.AddPolicy("CanUpdate", policy => policy.RequireClaim(Claims.Operation, "global.write"));
            options.AddPolicy("EditAuthPolicy", policy => policy.Requirements.Add(new MatchingOwnerRequirement()));
        });
    }

    private async Task EnsureRoles(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.EnsureRoleExists(
                   Roles.User,
                   new Claim(Claims.Operation, "global.read"),
                   new Claim(Claims.Operation, "global.write"));
        await roleManager.EnsureRoleExists(
                   Roles.Admin);

        await roleManager.EnsureRoleExists(
                   Roles.Bot,
                   new Claim(Claims.Operation, "global.read"));
    }

    private void ConfigureDI(IServiceCollection services, Config config)
    {
        services.AddSingleton(config);
        services.AddTransient<MatrixService>();
        services.AddTransient<BackupService>();
        services.AddTransient<AccountService>();
        services.AddTransient<UserServiceBase, UserService>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(
                Configuration.GetConnectionString("SqliteConnection")));
        switch (_config.MailerType)
        {
            case "mailgun":
                services.AddTransient<IMailerService, MailgunMailerService>();
                break;
            case "log-only":
                services.AddTransient<IMailerService, FakeMailerService>();
                break;
            default:
                throw new ConfigurationErrorsException($"Invalid mailer type: {Configuration["MailerType"]}");
        }
    }

    private static void ConfigureLocalization(IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                var supportedCultures = Config.SupportedLanguages.Select(l => new CultureInfo(l)).ToList();

                options.DefaultRequestCulture = new RequestCulture(
                    culture: _config.DefaultCulture,
                    uiCulture: _config.DefaultCulture);
                options.SetDefaultCulture(_config.DefaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(
                    new AcceptLanguageHeaderRequestCultureProvider());
            });
    }

    public void PushSerilogProperties(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("AspNetUserName", httpContext?.User?.Identity?.Name);
        diagnosticContext.Set("AspNetUserId", httpContext?.User?.GetUserId());
    }

    private static void OnAppStarted()
    {
        Log.Information("Application started at {@base}",
            _config.BaseUrl);
    }

    private static void OnAppStopping()
    {
        Log.Information("Application stopping");
    }

    private static void OnAppStopped()
    {
        Log.Information("Application stopped");
        Log.CloseAndFlush();
    }
}
