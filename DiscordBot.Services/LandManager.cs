using Discord;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    internal class LandManager : ILandManager
    {
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;

        public LandManager(IOptionsMonitor<DiscordConfiguration> discordConfiguration)
        {
            _discordConfiguration = discordConfiguration;
        }

        public UserLand? DetermineLand(IEnumerable<UserLand> lands, IEnumerable<ulong> roleIds, string? name)
        {
            if (name != null)
            {
                var land = lands.FirstOrDefault(x => x.Name == name);

                if (land == null)
                {
                    return null;
                }

                if(roleIds.Any(id => id == land.ManagerId))
                {
                    return land;
                }

                return null;
            }

            var all = lands.Where(x => roleIds.Any(id => id == x.ManagerId));

            if (!all.Any())
            {
                return null;
            }

            return all.First();
        }

        public async Task<IEnumerable<Embed>> GetMembersList(UserLand land, IGuild guild)
        {
            var maxMessageLength = _discordConfiguration.CurrentValue.MaxMessageLength;
            var embedList = new List<Embed>();
            var stringBuilder = new StringBuilder($"**Członkowie**: *{land.Name}*\n\n", 200);
            var users = await guild.GetUsersAsync(CacheMode.CacheOnly);

            var underlings = users.Where(x => x.RoleIds.Any(r => r == land.UnderlingId));

            foreach (var user in underlings)
            {
                var messageLength = stringBuilder.Length + user.Mention.Length;

                if (messageLength > maxMessageLength)
                {
                    embedList.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = stringBuilder.ToString(),
                    }.Build());
                    stringBuilder.Clear();
                }

                stringBuilder.AppendFormat("{0}\n", user.Mention);
            }

            embedList.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = stringBuilder.ToString()
            }.Build());

            return embedList;
        }
    }
}