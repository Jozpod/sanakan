using Discord.Commands;
using Sanakan.Services;
using Sanakan.Extensions;
using Sanakan.Services.Commands;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Discord.WebSocket;
using System.Linq;
using System;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;

namespace Sanakan.Modules
{
    [Name("Kraina"), RequireUserRole]
    public class LandsModule : ModuleBase<ICommandContext>
    {
        private readonly ILandManager _landManager;
        private readonly IGuildConfigRepository _guildConfigRepository;

        public ICommandContext Context { get; set; }

        public LandsModule(
            ILandManager landManager,
            IGuildConfigRepository guildConfigRepository)
        {
            _landManager = landManager;
            _guildConfigRepository = guildConfigRepository;
        }

        [Command("ludność", RunMode = RunMode.Async)]
        [Alias("ludnosc", "ludnośc", "ludnosć", "people")]
        [Summary("wyświetla użytkowników należących do krainy")]
        [Remarks("Kotleciki")]
        public async Task ShowPeopleAsync(
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            var user = Context.User as SocketGuildUser;

            if (user == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono uzytkownika!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.Roles, name);

            if (land == null)
            {
                await ReplyAsync("", embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var embed in await _landManager.GetMembersList(land, Context.Guild))
            {
                await ReplyAsync("", embed: embed);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        [Command("kraina dodaj", RunMode = RunMode.Async)]
        [Alias("land add")]
        [Summary("dodaje użytkownika do krainy")]
        [Remarks("Karna Kotleciki")]
        public async Task AddPersonAsync(
            [Summary("użytkownik")]SocketGuildUser userToAdd,
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            var user = Context.User as SocketGuildUser;

            if(user == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono uzytkownika!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.Roles, name);

            if (land == null)
            {
                await ReplyAsync("", embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var role = Context.Guild.GetRole(land.Underling);

            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli członka!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!userToAdd.Roles.Contains(role))
            {
                await userToAdd.AddRoleAsync(role);
            }

            var content = $"{userToAdd.Mention} dołącza do `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("kraina usuń", RunMode = RunMode.Async)]
        [Alias("land remove", "kraina usun")]
        [Summary("usuwa użytkownika z krainy")]
        [Remarks("Karna")]
        public async Task RemovePersonAsync(
            [Summary("użytkownik")]SocketGuildUser userToRemove,
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string? name = null)
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            var user = Context.User as SocketGuildUser;

            if (user == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono uzytkownika!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var land = _landManager.DetermineLand(config.Lands, user.Roles, name);

            if (land == null)
            {
                await ReplyAsync("", embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var role = guild.GetRole(land.Underling);
            if (role == null)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli członka!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (userToRemove.Roles.Contains(role))
            {
                await userToRemove.RemoveRoleAsync(role);
            }

            var content = $"{userToRemove.Mention} odchodzi z `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }
    }
}