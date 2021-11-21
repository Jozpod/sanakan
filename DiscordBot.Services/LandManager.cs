using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;

namespace Sanakan.DiscordBot.Services
{
    internal class LandManager : ILandManager
    {
        public MyLand? DetermineLand(IEnumerable<MyLand> lands, IEnumerable<ulong> roleIds, string? name)
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

        public async Task<List<Embed>> GetMembersList(MyLand land, IGuild guild)
        {
            var embedList = new List<Embed>();
            var temp = $"**Członkowie**: *{land.Name}*\n\n";
            var users = await guild.GetUsersAsync(CacheMode.CacheOnly);

            var underlings = users.Where(x => x.RoleIds.Any(r => r == land.UnderlingId));

            foreach (var user in underlings)
            {
                if (temp.Length + user.Mention.Length > 2000)
                {
                    embedList.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = temp
                    }.Build());
                    temp = $"{user.Mention}\n";
                }
                else temp += $"{user.Mention}\n";
            }

            embedList.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = temp
            }.Build());

            return embedList;
        }
    }
}