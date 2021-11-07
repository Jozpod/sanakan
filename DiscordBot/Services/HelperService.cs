﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

namespace Sanakan.Services
{
    internal class HelperService : IHelperService
    {
        private readonly IOptionsMonitor<DiscordConfiguration> _config;

        public IEnumerable<ModuleInfo> PublicModulesInfo { get; set; }
        public Dictionary<string, ModuleInfo> PrivateModulesInfo { get; set; }

        public HelperService(IOptionsMonitor<DiscordConfiguration> config)
        {
            _config = config;
            PublicModulesInfo = new List<ModuleInfo>();
            PrivateModulesInfo = new Dictionary<string, ModuleInfo>();
        }

        public string GivePublicHelp()
        {
            var commands = "**Lista poleceń:**\n";
            foreach (var item in GetInfoAboutModules(PublicModulesInfo))
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
            var item = GetInfoAboutModule(PrivateModulesInfo[moduleName]);
            return $"**Lista poleceń:**\n\n**{item.Prefix}:** " + string.Join("  ", item.Commands);
        }

        public string GiveHelpAboutPrivateCmd(string moduleName, string command, string prefix, bool throwEx = true)
        {
            var info = PrivateModulesInfo[moduleName];

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

        public string GetCommandInfo(CommandInfo cmd, string prefix = null)
        {
            var modulePrefix = GetModGroupPrefix(cmd.Module);
            var botPrefix = prefix ?? _config.CurrentValue.Prefix;

            var command = $"**{botPrefix}{modulePrefix}{cmd.Name}**";

            if (cmd.Parameters.Count > 0)
            {
                foreach (var param in cmd.Parameters)
                {
                    command += $" `{param.Name}` ";
                }
            }
                

            command += $" - {cmd.Summary}\n";

            if (cmd.Parameters.Count > 0)
            {
                foreach (var param in cmd.Parameters)
                {
                    command += $"*{param.Name}* - *{param.Summary}*\n";
                }
            }
                

            if (cmd.Aliases.Count > 1)
            {
                command += "\n**Aliasy:**\n";
                foreach (var alias in cmd.Aliases)
                    if (alias != cmd.Name) command += $"`{alias}` ";
            }

            command += $"\n\nnp. `{botPrefix}{modulePrefix}{cmd.Name} {cmd.Remarks}`";

            return command;
        }

        public string GiveHelpAboutPublicCmd(string command, string prefix, bool admin = false, bool dev = false)
        {
            foreach(var module in PublicModulesInfo)
            {
                var thisCommands = module.Commands.FirstOrDefault(x => x.Name == command);

                if (thisCommands == null)
                    thisCommands = module.Commands.FirstOrDefault(x => x.Aliases.Any(c => c == command));

                if (thisCommands != null)
                    return GetCommandInfo(thisCommands, prefix);
            }

            if (admin)
            {
                var res = GiveHelpAboutPrivateCmd("Moderacja", command, prefix, false);
                if (!string.IsNullOrEmpty(res)) return res;
            }

            if (dev)
            {
                var res = GiveHelpAboutPrivateCmd("Debug", command, prefix, false);
                if (!string.IsNullOrEmpty(res)) return res;
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

                var subMInfo = new SanakanSubModuleInfo()
                {
                    Prefix = GetModGroupPrefix(item, false),
                    Commands = new List<string>()
                };

                foreach (var cmd in item.Commands)
                    if (!string.IsNullOrEmpty(cmd.Name))
                        subMInfo.Commands.Add("`" + cmd.Name + "`");

                mInfo.Modules.Add(subMInfo);
            }
            return moduleInfos;
        }

        private SanakanSubModuleInfo GetInfoAboutModule(ModuleInfo module)
        {
            var subMInfo = new SanakanSubModuleInfo()
            {
                Prefix = module.Name,
                Commands = new List<string>()
            };

            foreach (var commands in module.Commands)
            {
                if (!string.IsNullOrEmpty(commands.Name))
                {
                    var name = "`" + commands.Name + "`";
                    if (!subMInfo.Commands.Contains(name))
                    {
                        subMInfo.Commands.Add(name);
                    }
                }
            }

            return subMInfo;
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

        public IEmbed GetInfoAboutUser(SocketGuildUser user)
        {
            return new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder().WithUser(user),
                ThumbnailUrl = user.GetUserOrDefaultAvatarUrl(),
                Fields = GetInfoUserFields(user),
                Color = EMType.Info.Color(),
            }.Build();
        }

        private List<EmbedFieldBuilder> GetInfoUserFields(SocketGuildUser user)
        {
            string roles = "Brak";
            if (user.Roles.Count > 1)
            {
                roles = "";
                foreach (var item in user.Roles.OrderByDescending(x => x.Position))
                    if (!item.IsEveryone)
                        roles += $"{item.Mention}\n";
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
                    Name = $"Role[{user.Roles.Count - 1}]",
                    Value = roles,
                    IsInline = false
                }
            };
        }

        public async Task<IEmbed> GetInfoAboutServerAsync(IGuild guild)
        {
            var author = new EmbedAuthorBuilder().WithName(guild.Name);
            if (guild.IconUrl != null)
            {
                author.WithIconUrl(guild.IconUrl);
            }

            var fields = await GetInfoGuildFieldsAsync(guild);

            var embed = new EmbedBuilder
            {
                Fields = fields,
                Color = EMType.Info.Color(),
                Author = author,
            };

            if (guild.IconUrl != null)
            {
                embed.WithThumbnailUrl(guild.IconUrl);
            }

            return embed.Build();
        }

        private async Task<List<EmbedFieldBuilder>> GetInfoGuildFieldsAsync(IGuild guild)
        {
            var roles = new StringBuilder(100);
            var owner = await guild.GetOwnerAsync();
            var users = await guild.GetUsersAsync();
            var channels = await guild.GetChannelsAsync();
            var textChannelsCount = channels.OfType<ITextChannel>().Count();
            var voiceChannelsCount = channels.OfType<IVoiceChannel>().Count();

            foreach (var item in guild.Roles.OrderByDescending(x => x.Position))
            {
                var isEveryone = item.Id == guild.Id;
                if (isEveryone && !ulong.TryParse(item.Name, out var id))
                {
                    roles.AppendFormat("{0} ", item.Mention);
                }
            }

            return new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Id",
                    Value = guild.Id,
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
                    Name = $"Role[{guild.Roles.Count}]",
                    Value = roles.ToString().ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                    IsInline = false
                }
            };
        }

        public async Task<IMessage> FindMessageInGuildAsync(SocketGuild guild, ulong id)
        {
            IMessage? msg = null;
            foreach (ITextChannel channel in guild.Channels)
                if (channel != null)
                {
                    msg = await channel.GetMessageAsync(id);
                    if (msg != null)
                        break;
                }

            return msg;
        }

        public IEmbed BuildRaportInfo(IMessage message, string reportAuthor, string reason, ulong reportId)
        {
            string attach = "brak";
            if (message.Attachments.Count > 0)
            {
                attach = "";
                foreach (var att in message.Attachments)
                    attach += $"{att.Url}\n";
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