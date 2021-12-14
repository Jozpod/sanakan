using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Preconditions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Kraina"), RequireUserRole]
    public class LandsModule : SanakanModuleBase
    {
        private readonly ILandManager _landManager;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public LandsModule(
            ILandManager landManager,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _landManager = landManager;
            _taskManager = taskManager;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("ludność", RunMode = RunMode.Async)]
        [Alias("ludnosc", "ludnośc", "ludnosć", "people", "population")]
        [Summary("wyświetla użytkowników należących do krainy")]
        [Remarks("Kotleciki")]
        public async Task ShowPeopleAsync(
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            var user = Context.User as IGuildUser;

            if (user == null)
            {
                await ReplyAsync(embed: "Nie odnaleziono uzytkownika!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                await ReplyAsync(embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var embed in await _landManager.GetMembersList(land, guild))
            {
                await ReplyAsync(embed: embed);
                await _taskManager.Delay(TimeSpan.FromSeconds(2));
            }
        }

        [Command("kraina dodaj", RunMode = RunMode.Async)]
        [Alias("land add")]
        [Summary("dodaje użytkownika do krainy")]
        [Remarks("Karna Kotleciki")]
        public async Task AddPersonAsync(
            [Summary("użytkownik")] IGuildUser userToAdd,
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            var user = Context.User as IGuildUser;

            if(user == null)
            {
                await ReplyAsync(embed: Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                await ReplyAsync(embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var role = guild.GetRole(land.UnderlingId);

            if (role == null)
            {
                await ReplyAsync(embed: "Nie odnaleziono roli członka!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!userToAdd.RoleIds.Contains(role.Id))
            {
                await userToAdd.AddRoleAsync(role);
            }

            var content = $"{userToAdd.Mention} dołącza do `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("kraina usuń", RunMode = RunMode.Async)]
        [Alias("land remove", "kraina usun")]
        [Summary("usuwa użytkownika z krainy")]
        [Remarks("Karna")]
        public async Task RemovePersonAsync(
            [Summary("użytkownik")] IGuildUser userToRemove,
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            var user = Context.User as IGuildUser;

            if (user == null)
            {
                await ReplyAsync(embed: Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                await ReplyAsync(embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var role = guild.GetRole(land.UnderlingId);
            if (role == null)
            {
                await ReplyAsync(embed: "Nie odnaleziono roli członka!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (userToRemove.RoleIds.Contains(role.Id))
            {
                await userToRemove.RemoveRoleAsync(role);
            }

            var content = $"{userToRemove.Mention} odchodzi z `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }
    }
}