using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Discord.Commands;
using System.Linq;
using System;
using Sanakan.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
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
                var modules = _helperService.GetPublicModules();
                var result = new Commands
                {
                    Prefix = _config.CurrentValue.Prefix,
                    Modules = GetInfoAboutModules(modules),
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

        private List<Module> GetInfoAboutModules(IEnumerable<ModuleInfo> moduleInfos)
        {
            var listOfModules = new List<Module>();
            foreach (var moduleInfo in moduleInfos)
            {
                var summaryModuleInfo = new Module
                {
                    Name = moduleInfo.Name,
                    SubModules = new List<SubModule>()
                };

                if (listOfModules.Any(x => x.Name.Equals(moduleInfo.Name)))
                {
                    summaryModuleInfo = listOfModules.First(x => x.Name.Equals(moduleInfo.Name));
                }
                else
                {
                    listOfModules.Add(summaryModuleInfo);
                }

                var alias = moduleInfo.Aliases.FirstOrDefault();

                var subMInfo = new SubModule()
                {
                    Prefix = alias,
                    Commands = new List<Command>(),
                    PrefixAliases = new List<string>()
                };

                foreach(var ali in moduleInfo.Aliases)
                {
                    if (!ali.Equals(subMInfo.Prefix))
                    {
                        subMInfo.PrefixAliases.Add(ali);
                    }
                }

                foreach (var command in moduleInfo.Commands)
                {
                    if (string.IsNullOrEmpty(command.Name))
                    {
                        continue;
                    }

                    var commandSummary = new Command()
                    {
                        Name = command.Name,
                        Example = command.Remarks,
                        Description = command.Summary,
                        Aliases = new List<string>(),
                        Attributes = new List<Api.Models.CommandAttribute>(),
                    };

                    foreach (var parameter in command.Parameters)
                    {
                        commandSummary.Attributes.Add(new Api.Models.CommandAttribute
                        {
                            Name = parameter.Name,
                            Description = parameter.Summary
                        });
                    }

                    foreach (var commandAlias in command.Aliases)
                    {
                        var parsedAlias = GetBoreboneCommandAlias(subMInfo.PrefixAliases, subMInfo.Prefix, commandAlias);
                        if (!commandSummary.Aliases.Any(x => x.Equals(parsedAlias)))
                        {
                            commandSummary.Aliases.Add(parsedAlias);
                        }
                    }
                    subMInfo.Commands.Add(commandSummary);
                }
                summaryModuleInfo.SubModules.Add(subMInfo);
            }
            return listOfModules;
        }

        private string GetBoreboneCommandAlias(List<string> moduleAli, string modulePrex, string commandAlias)
        {
            if (!string.IsNullOrEmpty(modulePrex))
            {
                commandAlias = commandAlias.Replace(modulePrex + " ", "");
            }

            foreach (var ali in moduleAli)
            {
                commandAlias = commandAlias.Replace(ali + " ", "");
            }

            return commandAlias;
        }
    }
}