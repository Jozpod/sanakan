using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class LandManager : ILandManager
    {
        public MyLand? DetermineLand(IEnumerable<MyLand> lands, IEnumerable<SocketRole> roles, string name)
        {            
            if (name != null)
            {
                var land = lands.FirstOrDefault(x => x.Name == name);

                if (land == null)
                {
                    return null;
                }

                if(roles.Any(x => x.Id == land.ManagerId))
                {
                    return land;
                }

                return null;
            }

            var all = lands.Where(x => roles.Any(c => c.Id == x.ManagerId));
            
            if (!all.Any())
            {
                return null;
            }
            
            
            return all.First();
        }

        public async Task<List<Embed>> GetMembersList(MyLand land, IGuild guild)
        {
            var embs = new List<Embed>();
            var temp = $"**Członkowie**: *{land.Name}*\n\n";
            var users = await guild.GetUsersAsync(CacheMode.CacheOnly);

            var underlings = users.Where(x => x.RoleIds.Any(r => r == land.UnderlingId));

            foreach (var user in underlings)
            {
                if (temp.Length + user.Mention.Length > 2000)
                {
                    embs.Add(new EmbedBuilder()
                    {
                        Color = EMType.Info.Color(),
                        Description = temp
                    }.Build());
                    temp = $"{user.Mention}\n";
                }
                else temp += $"{user.Mention}\n";
            }

            embs.Add(new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = temp
            }.Build());

            return embs;
        }
    }
}