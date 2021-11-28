using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    internal class HelperService : IHelperService
    {
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly ICollection<ModuleInfo> _publicModulesInfo;
        private readonly IDictionary<string, ModuleInfo> _privateModulesInfo;

        public HelperService(IOptionsMonitor<DiscordConfiguration> config)
        {
            _config = config;
            _publicModulesInfo = new List<ModuleInfo>();
            _privateModulesInfo = new Dictionary<string, ModuleInfo>();
        }

        public IEnumerable<ModuleInfo> GetPublicModules() => _publicModulesInfo;

        public void AddPublicModuleInfo(IEnumerable<ModuleInfo> moduleInfos)
        {
            foreach (var moduleInfo in moduleInfos)
            {
                _publicModulesInfo.Add(moduleInfo);
            }
        }

        public void AddPrivateModuleInfo(params (string, ModuleInfo)[] moduleInfos)
        {
            foreach (var (name, moduleInfo) in moduleInfos)
            {
                _privateModulesInfo.Add(name, moduleInfo);
            }
        }

        public string GivePublicHelp()
        {
            var commands = "**Lista poleceń:**\n";
            foreach (var item in GetInfoAboutModules(_publicModulesInfo))
            {
                var sSubInfo = new List<string>();
                foreach (var module in item.Modules)
                {
                    var info = "";
                    if (!string.IsNullOrWhiteSpace(module.Prefix))
                    {
                        info += $"      ***{module.Prefix}***";
                    }

                    sSubInfo.Add(info + " " + string.Join("  ", module.Commands));
                }

                commands += $"\r\n**{item.Name}:**" + string.Join("\n", sSubInfo);
            }
            commands += $"\r\n\r\nUżyj `{_config.CurrentValue.Prefix}pomoc [polecenie]`, aby uzyskać informacje dotyczące danego polecenia.";
            return commands;
        }

        public string GivePrivateHelp(string moduleName)
        {
            var item = GetInfoAboutModule(_privateModulesInfo[moduleName]);
            var stringBuilder = new StringBuilder($"**Lista poleceń:**\n\n**{item.Prefix}:** ", 500);

            foreach (var command in item.Commands)
            {
                stringBuilder.AppendFormat("{0}  ", command);
            }

            stringBuilder.Length -= 2;

            return stringBuilder.ToString();
        }

        public string? GiveHelpAboutPrivateCommand(string moduleName, string command, string prefix, bool throwEx = true)
        {
            if (!_privateModulesInfo.TryGetValue(moduleName, out var info))
            {
                return string.Empty;
            }

            var thisCommands = info.Commands.FirstOrDefault(x => x.Name == command);

            if (thisCommands == null)
            {
                thisCommands = info.Commands.FirstOrDefault(x => x.Aliases.Any(c => c == command));
            }

            if (thisCommands != null)
            {
                return GetCommandInfo(thisCommands, prefix);
            }

            if (throwEx)
            {
                throw new Exception("Polecenie nie istnieje!");
            }

            return null;
        }

        public string GetCommandInfo(CommandInfo commandInfo, string? prefix = null)
        {
            var modulePrefix = GetModGroupPrefix(commandInfo.Module);
            var botPrefix = prefix ?? _config.CurrentValue.Prefix;

            var stringBuilder = new StringBuilder($"**{botPrefix}{modulePrefix}{commandInfo.Name}**", 500);

            if (commandInfo.Parameters.Any())
            {
                foreach (var param in commandInfo.Parameters)
                {
                    stringBuilder.AppendFormat(" `{0}` ", param.Name);
                }
            }

            stringBuilder.AppendFormat(" - {0}\n", commandInfo.Summary);

            if (commandInfo.Parameters.Any())
            {
                foreach (var param in commandInfo.Parameters)
                {
                    stringBuilder.AppendFormat("*{0}* - *{0}*\n", param.Name, param.Summary);
                }
            }

            if (commandInfo.Aliases.Any())
            {
                stringBuilder.Append("\n**Aliasy:**\n");
                foreach (var alias in commandInfo.Aliases)
                {
                    if (alias != commandInfo.Name)
                    {
                        stringBuilder.AppendFormat("`{0}` ", alias);
                    }
                }
            }

            stringBuilder.AppendFormat("\n\nnp. `{0}{1}{2} {3}`", botPrefix, modulePrefix, commandInfo.Name, commandInfo.Remarks);

            return stringBuilder.ToString();
        }

        public string GiveHelpAboutPublicCommand(
            string command,
            string prefix,
            bool isAdmin = false,
            bool isDev = false)
        {
            foreach(var module in _publicModulesInfo)
            {
                var commands = module.Commands;

                var commandInfo = commands.FirstOrDefault(x => x.Name == command)
                    ?? commands.FirstOrDefault(x => x.Aliases.Any(c => c == command));

                if (commandInfo != null)
                {
                    return GetCommandInfo(commandInfo, prefix);
                }
            }

            if (isAdmin)
            {
                var info = GiveHelpAboutPrivateCommand(PrivateModules.Moderation, command, prefix, false);
                if (!string.IsNullOrEmpty(info))
                {
                    return info;
                }
            }

            if (isDev)
            {
                var info = GiveHelpAboutPrivateCommand(PrivateModules.Debug, command, prefix, false);
                if (!string.IsNullOrEmpty(info))
                {
                    return info;
                }
            }

            throw new Exception("Polecenie nie istnieje!");
        }

        private List<SanakanModuleInfo> GetInfoAboutModules(IEnumerable<ModuleInfo> modules)
        {
            var moduleInfos = new List<SanakanModuleInfo>();
            foreach (var item in modules)
            {
                var mInfo = new SanakanModuleInfo()
                {
                    Name = item.Name,
                    Modules = new List<SanakanSubModuleInfo>()
                };

                if (moduleInfos.Any(x => x.Name.Equals(item.Name)))
                {
                    mInfo = moduleInfos.First(x => x.Name.Equals(item.Name));
                }
                else moduleInfos.Add(mInfo);

                var sanakanSubModuleInfo = new SanakanSubModuleInfo()
                {
                    Prefix = GetModGroupPrefix(item, false),
                    Commands = new List<string>()
                };

                foreach (var command in item.Commands)
                {
                    if (!string.IsNullOrEmpty(command.Name))
                    {
                        sanakanSubModuleInfo.Commands.Add("`" + command.Name + "`");
                    }
                }
                    

                mInfo.Modules.Add(sanakanSubModuleInfo);
            }
            return moduleInfos;
        }

        private SanakanSubModuleInfo GetInfoAboutModule(ModuleInfo module)
        {
            var sanakanSubModuleInfo = new SanakanSubModuleInfo()
            {
                Prefix = module.Name,
                Commands = new List<string>()
            };

            foreach (var commands in module.Commands)
            {
                if (!string.IsNullOrEmpty(commands.Name))
                {
                    var name = "`" + commands.Name + "`";
                    if (!sanakanSubModuleInfo.Commands.Contains(name))
                    {
                        sanakanSubModuleInfo.Commands.Add(name);
                    }
                }
            }

            return sanakanSubModuleInfo;
        }

        private string GetModGroupPrefix(ModuleInfo mod, bool space = true)
        {
            string prefix = "";
            var att = mod.Aliases.FirstOrDefault();
            if (!string.IsNullOrEmpty(att))
            {
                if (space)
                {
                    att += " ";
                }
                prefix = att;
            }
            return prefix;
        }

        private struct SanakanModuleInfo
        {
            public string Name { get; set; }
            public List<SanakanSubModuleInfo> Modules { get; set; }
        }

        private struct SanakanSubModuleInfo
        {
            public string Prefix { get; set; }
            public List<string> Commands { get; set; }
        }

        public IEmbed GetInfoAboutUser(IGuildUser user)
        {
            return new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder().WithUser(user),
                ThumbnailUrl = user.GetUserOrDefaultAvatarUrl(),
                Fields = GetInfoUserFields(user),
                Color = EMType.Info.Color(),
            }.Build();
        }

        private List<EmbedFieldBuilder> GetInfoUserFields(IGuildUser user)
        {
            var rolesSummary = new StringBuilder("Brak", 200);
            var roleIds = user.RoleIds;

            if (roleIds.Any())
            {
                var guild = user.Guild;
                var guildRoles = guild.Roles;
                var userRoles = guildRoles
                    .Join(roleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                    .OrderByDescending(x => x.Position);

                foreach (var item in userRoles)
                {
                    var isEveryone = item.Id == guild.Id;

                    if (isEveryone)
                    {
                        continue;
                    }

                    rolesSummary.AppendFormat("{0}\n", item.Mention);
                }
            }

            return new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Id",
                    Value = user.Id,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Pseudo",
                    Value = user.Nickname ?? "Brak",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Status",
                    Value = user.Status.ToString(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Bot",
                    Value = user.IsBot ? "Tak" : "Nie",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Utworzono",
                    Value = user.CreatedAt.DateTime.ToString(),
                    IsInline = false
                },
                new EmbedFieldBuilder
                {
                    Name = "Dołączono",
                    Value = user.JoinedAt.ToString().Split('+')[0],
                    IsInline = false
                },
                new EmbedFieldBuilder
                {
                    Name = $"Role[{roleIds.Count - 1}]",
                    Value = rolesSummary.ToString(),
                    IsInline = false
                }
            };
        }

        public async Task<IEmbed> GetInfoAboutServerAsync(IGuild guild)
        {
            var author = new EmbedAuthorBuilder().WithName(guild.Name);
            var iconUrl = guild.IconUrl;

            if (iconUrl != null)
            {
                author.WithIconUrl(iconUrl);
            }

            var fields = await GetInfoGuildFieldsAsync(guild);

            var embed = new EmbedBuilder
            {
                Fields = fields,
                Color = EMType.Info.Color(),
                Author = author,
            };

            if (iconUrl != null)
            {
                embed.WithThumbnailUrl(iconUrl);
            }

            return embed.Build();
        }

        private async Task<List<EmbedFieldBuilder>> GetInfoGuildFieldsAsync(IGuild guild)
        {
            var guildId = guild.Id;
            var stringBuilder = new StringBuilder(100);
            var owner = await guild.GetOwnerAsync();
            var users = await guild.GetUsersAsync();
            var channels = await guild.GetChannelsAsync();
            var textChannelsCount = channels.OfType<ITextChannel>().Count();
            var voiceChannelsCount = channels.OfType<IVoiceChannel>().Count();
            var roles = guild.Roles;

            foreach (var item in roles.OrderByDescending(x => x.Position))
            {
                var isEveryone = item.Id == guildId;
                if (isEveryone && !ulong.TryParse(item.Name, out var id))
                {
                    stringBuilder.AppendFormat("{0} ", item.Mention);
                }
            }

            return new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Id",
                    Value = guildId,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Właściciel",
                    Value = owner.Mention,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Utworzono",
                    Value = guild.CreatedAt.DateTime.ToString(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Liczba użytkowników",
                    Value = users.Count,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Kanały tekstowe",
                    Value = textChannelsCount,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Kanały głosowe",
                    Value = voiceChannelsCount,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = $"Role[{roles.Count}]",
                    Value = stringBuilder.ToString().ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                    IsInline = false
                }
            };
        }

        public async Task<IMessage?> FindMessageInGuildAsync(IGuild guild, ulong id)
        {
            IMessage? message = null;
            foreach (var channel in await guild.GetTextChannelsAsync())
            {
                if (channel == null)
                {
                    continue;
                }

                message = await channel.GetMessageAsync(id);
                if (message != null)
                {
                    break;
                }
            }

            return message;
        }

        public IEmbed BuildRaportInfo(IMessage message, string reportAuthor, string reason, ulong reportId)
        {
            var attach = "brak";

            if (message.Attachments.Count > 0)
            {
                attach = "";
                foreach (var attachment in message.Attachments)
                {
                    attach += $"{attachment.Url}\n";
                }
            }

            return new EmbedBuilder
            {
                Footer = new EmbedFooterBuilder().WithText($"Zgłasza: {reportAuthor}".ElipseTrimToLength(EmbedFooterBuilder.MaxFooterTextLength)),
                Description = message.Content?.ElipseTrimToLength(1500) ?? "sam załącznik",
                Author = new EmbedAuthorBuilder().WithUser(message.Author),
                Color = EMType.Error.Color(),
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Kanał:",
                        Value = message.Channel.Name,
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Napisano:",
                        Value = $"{message.GetLocalCreatedAtShortDateTime()}"
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Id zgloszenia:",
                        Value = reportId
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = "Powód:",
                        Value = reason.ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength)
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = false,
                        Name = "Załączniki:",
                        Value = attach.ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength)
                    }
                }
            }.Build();
        }
    }
}