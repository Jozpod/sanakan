using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Sanakan.Api;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Common.Converters;
using Sanakan.Daemon.Builder;
using Sanakan.DAL.Builder;
using Sanakan.DiscordBot.Builder;
using Sanakan.DiscordBot.Services.Builder;
using Sanakan.DiscordBot.Session.Builder;
using Sanakan.DiscordBot.Supervisor;
using Sanakan.Game.Builder;
using Sanakan.ShindenApi.Builder;
using Sanakan.TaskQueue.Builder;
using Sanakan.Web;
using Sanakan.Web.Resources;
using Sanakan.Web.Swagger;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sanakan.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var jwtConfiguration = Configuration
                .GetSection("SanakanApi:Jwt")
                .Get<JwtConfiguration>();

            var localeConfiguration = Configuration
                .GetSection("Locale")
                .Get<LocaleConfiguration>();

            CultureInfo.DefaultThreadCurrentCulture = localeConfiguration.Language;
            CultureInfo.DefaultThreadCurrentUICulture = localeConfiguration.Language;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Issuer,
                    IssuerSigningKey = JwtBuilder.ToSecurityKey(jwtConfiguration.IssuerSigningKey),
                };
            });

            services.AddAuthorization(op =>
            {
                op.AddPolicy(RegisteredNames.Player, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();

                    policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == RegisteredNames.Player
                        && c.Value == RegisteredNames.WaifuPlayer));
                });

                op.AddPolicy(RegisteredNames.Site, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();

                    policy.RequireAssertion(context => !context.User.HasClaim(c => c.Type == RegisteredNames.Player));
                });
            });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                });
            services.AddCors(options =>
            {
                options.AddPolicy(RegisteredNames.AllowEverything, builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizationOperationFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sanakan API",
                    Version = "1.0",
                    Description = Strings.SwaggerDescription,
                });

                options.MapType(typeof(TimeSpan), () => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Sanakan.xml");

                if (File.Exists(filePath))
                {
                    options.IncludeXmlComments(filePath);
                }

                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddHttpContextAccessor();
            services.AddJwtBuilder();
            services.AddRequestBodyReader();
            services.AddUserContext();
            services.AddWritableOption<SanakanConfiguration>();
            services.AddDiscordBot();
            services.AddDiscordBotServices();
            services.AddSingleton(Encoding.UTF8);
            services.AddResourceManager();
            services.AddImageResources();
            services.AddFontResources();
            services.AddRandomNumberGenerator();
            services.AddFileSystem();
            services.AddSystemClock();
            services.AddOperatingSystem();
            services.AddTaskManager();
            services.AddRepositories();
            services.AddDatabaseFacade();
            services.AddTaskQueue();
            services.AddShindenApi();
            services.AddGameServices();
            services.AddTimer();
            services.AddSupervisorServices();
            services.AddSessionManager();
            services.AddCache(Configuration.GetSection("Cache"));
            services.AddConfiguration(Configuration);
            services.AddSanakanDbContext(Configuration);
            services.AddHostedServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseCors(RegisteredNames.AllowEverything);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(pr =>
            {
                pr.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Sanakan API";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
                options.DefaultModelsExpandDepth(-1);
                options.InjectStylesheet("/swagger-theme.css");
            });
        }
    }
}