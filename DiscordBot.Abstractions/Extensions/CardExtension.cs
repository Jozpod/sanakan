using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models;
using Sanakan.ShindenApi.Utilities;

namespace Sanakan.Extensions
{
    public static class CardExtension
    {
        public static string GetShortString(this Card card, bool nameAsUrl = false)
        {
            string name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            return $"**[{card.Id}]** {name} **{card.GetCardRealRarity()}**";
        }
        public static string GetStatusIcons(this Card card)
        {
            var icons = new List<string>();
            if (card.Active) icons.Add("☑️");
            if (card.Unique) icons.Add("💠");
            if (card.FromFigure) icons.Add("🎖️");
            if (!card.IsTradable) icons.Add("⛔");
            if (card.IsBroken) icons.Add("💔");
            if (card.InCage) icons.Add("🔒");
            if (card.Expedition != ExpeditionCardType.None) icons.Add("✈️");
            if (!string.IsNullOrEmpty(card.CustomImage)) icons.Add("🖼️");
            if (!string.IsNullOrEmpty(card.CustomBorder)) icons.Add("✂️");

            var value = card.GetThreeStateMarketValue();
            if (value == MarketValue.Low) icons.Add("♻️");
            if (value == MarketValue.High) icons.Add("💰");

            if (card.TagList.Count > 0)
            {
                if (card.TagList.Any(x => x.Name.Equals("ulubione", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("💗");

                if (card.TagList.Any(x => x.Name.Equals("galeria", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("📌");

                if (card.TagList.Any(x => x.Name.Equals("rezerwacja", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("📝");

                if (card.TagList.Any(x => x.Name.Equals("wymiana", StringComparison.CurrentCultureIgnoreCase)))
                    icons.Add("🔄");
            }
            return string.Join(" ", icons);
        }
        public static string GetString(
            this Card card,
            bool withoutId = false,
            bool withUpgrades = false,
            bool nameAsUrl = false,
            bool allowZero = false,
            bool showBaseHp = false)
        {
            var idStr = withoutId ? "" : $"**[{card.Id}]** ";
            var name = nameAsUrl ? card.GetNameWithUrl() : card.Name;
            var upgradeCount = (withUpgrades && !card.FromFigure) ? $"_(U:{card.UpgradesCount})_" : "";

            return $"{idStr} {name} **{card.GetCardRealRarity()}** {card.GetCardParams(showBaseHp, allowZero)} {upgradeCount}";
        }
        public static string GetCharacterUrl(this Card card) => UrlHelpers.GetCharacterURL(card.CharacterId);
        public static string GetNameWithUrl(this Card card) => $"[{card.Name}]({card.GetCharacterUrl()})";
        public static string GetDesc(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"{card.GetNameWithUrl()} **{card.GetCardRealRarity()}**\n"
                + $"*{card.Title ?? "????"}*\n\n"
                + $"*{card.GetCardParams(true, false, true)}*\n\n"
                + $"**Relacja:** {card.GetAffectionString()}\n"
                + $"**Doświadczenie:** {card.ExpCount.ToString("F")}/{card.ExpToUpgrade().ToString("F")}\n"
                + $"**Dostępne ulepszenia:** {card.UpgradesCount}\n\n"
                + $"**W klatce:** {card.InCage.GetYesNo()}\n"
                + $"**Aktywna:** {card.Active.GetYesNo()}\n"
                + $"**Możliwość wymiany:** {card.IsTradable.GetYesNo()}\n\n"
                + $"**WID:** {card.Id} *({card.CharacterId})*\n"
                + $"**Restarty:** {card.RestartCount}\n"
                + $"**Pochodzenie:** {card.Source.GetString()}\n"
                + $"**Tagi:** {tags}\n"
                + $"{card.GetStatusIcons()}\n\n";
        }
        public static string GetDescSmall(this Card card)
        {
            var tags = string.Join(" ", card.TagList.Select(x => x.Name));
            if (card.TagList.Count < 1) tags = "---";

            return $"**[{card.Id}]** *({card.CharacterId})*\n"
                + $"{card.GetString(true, true, true, false, true)}\n"
                + $"_{card.Title}_\n\n"
                + $"{card.Dere}\n"
                + $"{card.GetAffectionString()}\n"
                + $"{card.ExpCount.ToString("F")}/{card.ExpToUpgrade().ToString("F")} exp\n\n"
                + $"{tags}\n"
                + $"{card.GetStatusIcons()}";
        }
    }
}