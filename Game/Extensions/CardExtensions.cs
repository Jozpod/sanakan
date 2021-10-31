using Sanakan.DAL.Models;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Game.Extensions
{
    public static class CardExtensions
    {
        public static string GetCharacterUrl(this Card card) => UrlHelpers.GetCharacterURL(card.CharacterId);
        public static string GetNameWithUrl(this Card card) => $"[{card.Name}]({card.GetCharacterUrl()})";
        public static string GetString(
          this Card card,
          bool withoutId = false,
          bool withUpgrades = false,
          bool nameAsUrl = false,
          bool allowZero = false,
          bool showBaseHp = false)
        {
            string idStr = withoutId ? "" : $"**[{card.Id}]** ";
            string name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            string upgCnt = (withUpgrades && !card.FromFigure) ? $"_(U:{card.UpgradesCount})_" : "";

            return $"{idStr} {name} **{card.GetCardRealRarity()}** {card.GetCardParams(showBaseHp, allowZero)} {upgCnt}";
        }
    }
}
