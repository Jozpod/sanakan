using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Game.Extensions
{
    public static class UserExtensions
    {
        public static string GetViewValueForTop(this User user, TopType type)
        {
            switch (type)
            {
                default:
                case TopType.Level:
                    return $"{user.Level} **LVL** ({user.ExperienceCount} **EXP**)";

                case TopType.ScCnt:
                    return $"{user.ScCount} **SC**";

                case TopType.TcCnt:
                    return $"{user.TcCount} **TC**";

                case TopType.AcCnt:
                    return $"{user.AcCount} **AC**";

                case TopType.PcCnt:
                    return $"{user.GameDeck.PVPCoins} **PC**";

                case TopType.Posts:
                    return $"{user.MessagesCount}";

                case TopType.PostsMonthly:
                    return $"{user.MessagesCount - user.MessagesCountAtDate}";

                case TopType.PostsMonthlyCharacter:
                    return $"{user.CharacterCountFromDate / (user.MessagesCount - user.MessagesCountAtDate)}";

                case TopType.Commands:
                    return $"{user.CommandsCount}";

                case TopType.Card:
                    return user.GameDeck.Cards
                        .OrderByDescending(x => x.CardPower)?
                        .FirstOrDefault()?.GetString(false, false, true) ?? "---";

                case TopType.Cards:
                    return $"{user.GameDeck.Cards.Count}";

                case TopType.CardsPower:
                    return user.GameDeck.GetCardCountStats();

                case TopType.Karma:
                case TopType.KarmaNegative:
                    return $"{user.GameDeck.Karma.ToString("F")}";

                case TopType.Pvp:
                    return $"{user.GameDeck.GlobalPVPRank}";

                case TopType.PvpSeason:
                    return $"{user.GameDeck.SeasonalPVPRank}";
            }
        }
    }
}
