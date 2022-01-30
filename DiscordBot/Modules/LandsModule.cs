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
            [Summary(ParameterInfo.Land)][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedById(guild.Id);
            var user = Context.User as IGuildUser;
            Embed embed;

            if (user == null)
            {
                embed = Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                embed = Strings.YouDontManageLand.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            foreach (var embedIt in await _landManager.GetMembersList(land, guild))
            {
                await ReplyAsync(embed: embedIt);
                await _taskManager.Delay(TimeSpan.FromSeconds(2));
            }
        }

        [Command("kraina dodaj", RunMode = RunMode.Async)]
        [Alias("land add")]
        [Summary("dodaje użytkownika do krainy")]
        [Remarks("Karna Kotleciki")]
        public async Task AddPersonAsync(
            [Summary(ParameterInfo.User)] IGuildUser userToAdd,
            [Summary(ParameterInfo.Land)][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedById(guild.Id);
            var user = Context.User as IGuildUser;
            Embed embed;

            if (user == null)
            {
                await ReplyAsync(embed: Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                embed = Strings.YouDontManageLand.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var role = guild.GetRole(land.UnderlingId);

            if (role == null)
            {
                embed = Strings.MemberRoleNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!userToAdd.RoleIds.Contains(role.Id))
            {
                await userToAdd.AddRoleAsync(role);
            }

            embed = $"{userToAdd.Mention} dołącza do `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("kraina usuń", RunMode = RunMode.Async)]
        [Alias("land remove", "kraina usun")]
        [Summary("usuwa użytkownika z krainy")]
        [Remarks("Karna")]
        public async Task RemovePersonAsync(
            [Summary(ParameterInfo.User)] IGuildUser userToRemove,
            [Summary(ParameterInfo.Land)][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedById(guild.Id);
            var user = Context.User as IGuildUser;
            Embed embed;

            if (user == null)
            {
                embed = Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.RoleIds, name);

            if (land == null)
            {
                embed = Strings.YouDontManageLand.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var role = guild.GetRole(land.UnderlingId);
            if (role == null)
            {
                embed = Strings.MemberRoleNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (userToRemove.RoleIds.Contains(role.Id))
            {
                await userToRemove.RemoveRoleAsync(role);
            }

            embed = $"{userToRemove.Mention} odchodzi z `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }
    }
}