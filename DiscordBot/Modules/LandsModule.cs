using Discord.Commands;
using Sanakan.Services;
using Sanakan.Extensions;
using Sanakan.Services.Commands;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Discord.WebSocket;
using System.Linq;
using System;
using DAL.Repositories.Abstractions;

namespace Sanakan.Modules
{
    [Name("Kraina"), RequireUserRole]
    public class LandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly LandManager _manager;
        private readonly IUserRepository _userRepository;
        private readonly IRepository _repository;

        public LandsModule(
            LandManager manager,
            IRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        [Command("ludność", RunMode = RunMode.Async)]
        [Alias("ludnosc", "ludnośc", "ludnosć", "people")]
        [Summary("wyświetla użytkowników należących do krainy")]
        [Remarks("Kotleciki")]
        public async Task ShowPeopleAsync([Summary("nazwa krainy (opcjonalne)")][Remainder]string name = null)
        {
            var config = await _repository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var land = _manager.DetermineLand(config.Lands, Context.User as SocketGuildUser, name);

            if (land == null)
            {
                await ReplyAsync("", embed: "Nie zarządzasz żadną krainą.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var emb in _manager.GetMembersList(land, Context.Guild))
            {
                await ReplyAsync("", embed: emb);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        [Command("kraina dodaj", RunMode = RunMode.Async)]
        [Alias("land add")]
        [Summary("dodaje użytkownika do krainy")]
        [Remarks("Karna Kotleciki")]
        public async Task AddPersonAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("nazwa krainy (opcjonalne)")][Remainder]string name = null)
        {
            var config = await _repository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var land = _manager.DetermineLand(config.Lands, Context.User as SocketGuildUser, name);

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

            if (!user.Roles.Contains(role))
            {
                await user.AddRoleAsync(role);
            }

            var content = $"{user.Mention} dołącza do `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("kraina usuń", RunMode = RunMode.Async)]
        [Alias("land remove", "kraina usun")]
        [Summary("usuwa użytkownika z krainy")]
        [Remarks("Karna")]
        public async Task RemovePersonAsync([Summary("użytkownik")]SocketGuildUser user, [Summary("nazwa krainy (opcjonalne)")][Remainder]string name = null)
        {
            var guild = Context.Guild;
            var config = await _repository.GetCachedGuildFullConfigAsync(guild.Id);
            var land = _manager.DetermineLand(config.Lands, Context.User as SocketGuildUser, name);

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

            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
            }

            var content = $"{user.Mention} odchodzi z `{land.Name}`.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }
    }
}