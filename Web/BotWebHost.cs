#pragma warning disable 1591

using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.Text;
using Sanakan.Config;
using Microsoft.AspNetCore.Mvc;
using Sanakan.Services.Executor;
using Discord.WebSocket;
using Shinden;
using Sanakan.Services.PocketWaifu;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace Sanakan.Api
{
    public static class BotWebHost
    {
        public static void RunWebHost(DiscordSocketClient client, ShindenClient shinden, Waifu waifu, IConfig config, Services.Helper helper, IExecutor executor, Shinden.Logger.ILogger logger)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                CreateWebHostBuilder(config).ConfigureServices(services =>
                {
                    services.AddSingleton(waifu);
                    services.AddSingleton(logger);
                    services.AddSingleton(client);
                    services.AddSingleton(helper);
                    services.AddSingleton(shinden);
                    services.AddSingleton(executor);
                }).Build().Run();
            }).Start();
        }
}