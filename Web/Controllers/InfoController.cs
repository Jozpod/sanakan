using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Discord.Commands;
using System.Linq;
using System;
using Sanakan.Services;
using Sanakan.Config;
using Sanakan.Extensions;
using Sanakan.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Sanakan.Web.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Common.Configuration;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InfoController : ControllerBase
    {
        private readonly IHelperService _helperService;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;

        public InfoController(
            IHelperService helperService,
            IOptionsMonitor<DiscordConfiguration> config)
        {
            _helperService = helperService;
            _config = config;
        }

        /// <summary>
        /// Gets the list of discord bot commands.
        /// </summary>
        [HttpGet("commands")]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Commands), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommandsInfoAsync()
        {
            try
            {
                var result = new Commands
                {
                    Prefix = _config.CurrentValue.Prefix,
                    Modules = GetInfoAboutModules(_helperService.PublicModulesInfo)
                };

                return Ok(result);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 500,
                };
            }
        }

        private List<Module> GetInfoAboutModules(IEnumerable<ModuleInfo> modules)
        {
            var mod = new List<Module>();
            foreach (var item in modules)
            {
                var mInfo = new Module
                {
                    Name = item.Name,
                    SubModules = new List<SubModule>()
                };

                if (mod.Any(x => x.Name.Equals(item.Name)))
                {
                    mInfo = mod.First(x => x.Name.Equals(item.Name));
                }
                else
                {
                    mod.Add(mInfo);
                }

                var subMInfo = new SubModule()
                {
                    Prefix = item.Aliases.FirstOrDefault(),
                    Commands = new List<Command>(),
                    PrefixAliases = new List<string>()
                };

                foreach(var ali in item.Aliases)
                {
                    if (!ali.Equals(subMInfo.Prefix))
                        subMInfo.PrefixAliases.Add(ali);
                }

                foreach (var cmd in item.Commands)
                {
                    if (!string.IsNullOrEmpty(cmd.Name))
                    {
                        var cc = new Command()
                        {
                            Name = cmd.Name,
                            Example = cmd.Remarks,
                            Description = cmd.Summary,
                            Aliases = new List<string>(),
                            Attributes = new List<Api.Models.CommandAttribute>(),
                        };

                        foreach(var atr in cmd.Parameters)
                        {
                            cc.Attributes.Add(new Api.Models.CommandAttribute
                            {
                                Name = atr.Name,
                                Description = atr.Summary
                            });
                        }

                        foreach(var al in cmd.Aliases)
                        {
                            var alss = GetBoreboneCmdAlias(subMInfo.PrefixAliases, subMInfo.Prefix, al);
                            if (!cc.Aliases.Any(x => x.Equals(alss)))
                                cc.Aliases.Add(alss);
                        }
                        subMInfo.Commands.Add(cc);
                    }
                }
                mInfo.SubModules.Add(subMInfo);
            }
            return mod;
        }

        private string GetBoreboneCmdAlias(List<string> moduleAli, string modulePrex, string cmdAlias)
        {
            if (!string.IsNullOrEmpty(modulePrex))
                cmdAlias = cmdAlias.Replace(modulePrex + " ", "");

            foreach(var ali in moduleAli)
                cmdAlias  = cmdAlias.Replace(ali + " ", "");

            return cmdAlias;
        }
    }
}