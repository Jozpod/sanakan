using Discord.WebSocket;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Builder;
using Sanakan.DiscordBot.Services;
using Sanakan.Game.Builder;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.PocketWaifu;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Builder;
using Sanakan.TaskQueue.Builder;
using Sanakan.Web.HostedService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetSection("Jwt.Issuer").Value,
                    ValidAudience = Configuration.GetSection("Jwt.Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt.Key").Value))
                };
            });

            services.AddAuthorization(op =>
            {
                op.AddPolicy("Player", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();

                    policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == "Player" 
                        && c.Value == "waifu_player"));
                });

                op.AddPolicy("Site", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();

                    policy.RequireAssertion(context => !context.User.HasClaim(c => c.Type == "Player"));
                });
            });

            services.AddControllers()
                .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(o => o.SerializerSettings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() }));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowEverything", builder =>
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Sanakan API",
                    Version = "1.0",
                    Description = "Autentykacja następuje poprzez dopasowanie tokenu przesłanego w ciele zapytania `api/token`, a następnie wysyłania w nagłowku `Authorization` z przedrostkiem `Bearer` otrzymanego w zwrocie tokena."
                        + "\n\nDocelowa wersja api powinna zostać przesłana pod nagówkiem `x-api-version`, w przypadku jej nie podania zapytania są interpretowane jako wysłane do wersji `1.0`.",
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Sanakan.xml");

                if (File.Exists(filePath))
                {
                    c.IncludeXmlComments(filePath);
                }
                

                c.CustomSchemaIds(x => x.FullName);
            });
            
            services.AddDiscordBotServices();

            services.AddSingleton(pr => {
                return new DiscordSocketClient(new DiscordSocketConfig()
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                });
            });
            services.AddSingleton(Encoding.UTF8);
            services.AddResourcekManager();
            services.AddRandomNumberGenerator();
            services.AddFileSystem();
            services.AddSystemClock();
            services.AddOperatingSystem();
            services.AddTaskManager();
            services.AddRepositories();
            services.AddTaskQueue();
            services.AddShindenApi();
            services.AddGameServices();
            services.AddCache(Configuration.GetSection("Cache"));
            services.Configure<SanakanConfiguration>(Configuration);
            services.Configure<DaemonsConfiguration>(Configuration.GetSection("Daemons"));
            services.Configure<DatabaseConfiguration>(Configuration.GetSection("Database"));
            services.Configure<LocaleConfiguration>(Configuration.GetSection("Locale"));
            services.Configure<DiscordConfiguration>(Configuration.GetSection("Discord"));
            services.Configure<ShindenApiConfiguration>(Configuration.GetSection("ShindenApi"));
            services.Configure<ApiConfiguration>(Configuration.GetSection("ShindenApi"));
            services.Configure<ExperienceConfiguration>(Configuration.GetSection("Experience"));
            services.Configure<ApiConfiguration>(Configuration.GetSection("SanakanApi"));
            services.Configure<List<RichMessageConfig>>(Configuration.GetSection("RMConfig"));
            services.AddHostedService<TaskQueueHostedService>();
            services.AddHostedService<MemoryUsageHostedService>();
            services.AddHostedService<SessionHostedService>();
            services.AddHostedService<ProfileHostedService>();
            services.AddHostedService<SupervisorHostedService>();
            services.AddHostedService<ModeratorHostedService>();
            services.AddHostedService<ChaosHostedService>();
            services.AddHostedService<DiscordBotHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseCors("AllowEverything");
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor 
                    | ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
