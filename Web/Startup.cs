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
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Services;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.PocketWaifu;
using Sanakan.ShindenApi;
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
            //var tmpCnf = config.Get();
            //services.AddSingleton(config);

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

            //services.AddSingleton<IExecutor>(_executor);
            //services.AddSingleton(_sessions);
            //services.AddSingleton(_profile);
            //services.AddSingleton(_config);
            //services.AddSingleton(_client);
            //services.AddSingleton(_helper);
            //services.AddSingleton(_events);
            //services.AddSingleton(_chaos);
            //services.AddSingleton(_waifu);
            //services.AddSingleton(_spawn);
            //services.AddSingleton(_mod);
            services.AddSingleton<IShindenClient, ShindenClient>();
            //services.AddSingleton<IShindenClient, ShindenClient>(pr =>
            //{
            //    return new ShindenClient(
            //        new Auth(tmpCnf.Shinden.Token,
            //        tmpCnf.Shinden.UserAgent,
            //        tmpCnf.Shinden.Marmolade),
            //    _logger);
            //});
            
            services.AddSingleton<HelperService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<IImageProcessing, ImageProcessing>();
            services.AddSingleton<IWaifuService, WaifuService>();
            //_shindenClient =

            services.AddSingleton(pr => {
                return new DiscordSocketClient(new DiscordSocketConfig()
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                });
            });
            services.AddSingleton(Encoding.UTF8);
            services.AddSingleton<Services.Shinden>();
            services.AddSingleton<Services.LandManager>();
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
            services.AddSingleton<IDiscordSocketClientAccessor, DiscordSocketClientAccessor>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddHostedService<DiscordBotHostedService>();
            services.AddHostedService<MemoryUsageHostedService>();
            services.AddHostedService<DiscordBotHostedService>();
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
