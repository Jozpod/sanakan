using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models.Configuration;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class LandManager
    {
        public MyLand? DetermineLand(IEnumerable<MyLand> lands, SocketGuildUser user, string name)
        {
            if (user == null)
            {
                return null;
            }
            
            if (name != null)
            {
                var land = lands.FirstOrDefault(x => x.Name == name);

                if (land == null)
                {
                    return null;
                }

                if(user.Roles.Any(x => x.Id == land.Manager))
                {
                    return land;
                }

                return null;
            }

            var all = lands.Where(x => user.Roles.Any(c => c.Id == x.Manager));
            
            if (all.Count() < 1)
            {
                return null;
            }
            
            
            return all.First();
        }

        public List<Embed> GetMembersList(MyLand land, SocketGuild guild)
        {
            var embs = new List<Embed>();
            string temp = $"**Członkowie**: *{land.Name}*\n\n";
            var underlings = guild.Users.Where(x => x.Roles.Any(r => r.Id == land.Underling));

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