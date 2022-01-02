using Sanakan.Common;
using Sanakan.Common.Extensions;
using Sanakan.DAL.Models;
using Sanakan.ShindenApi.Utilities;
using System.Linq;
using System.Text;

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
            var value = card.GetThreeStateMarketValue();
            var result = new StringBuilder(20);
            var metaData = new[]
            {
                (card.Active, "☑️"),
                (card.IsUnique, "💠"),
                (card.FromFigure, "🎖️"),
                (!card.IsTradable, "⛔"),
                (card.IsBroken, "💔"),
                (card.InCage, "🔒"),
                (card.Expedition != ExpeditionCardType.None, "✈️"),
                (card.CustomImageUrl != null, "🖼️"),
                (card.CustomBorderUrl != null, "✂️"),
                (value == MarketValue.Low, "♻️"),
                (value == MarketValue.High, "💰")
            };

            foreach (var (flag, icon) in metaData)
            {
                if (flag)
                {
                    result.AppendFormat("{0} ", icon);
                }
            }

            foreach (var tagItem in card.Tags)
            {
                var tagName = tagItem.Name.ToLowerInvariant();
                switch (tagName)
                {
                    case Tags.Favourite:
                        result.AppendFormat("{0} ", "💗");
                        break;
                    case Tags.Gallery:
                        result.AppendFormat("{0} ", "📌");
                        break;
                    case "rezerwacja":
                        result.AppendFormat("{0} ", "📝");
                        break;
                    case "wymiana":
                        result.AppendFormat("{0} ", "🔄");
                        break;
                    default:
                        break;
                }
            }

            return result.ToString();
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
            var tags = string.Join(" ", card.Tags.Select(x => x.Name));
            if (card.Tags.Count < 1)
            {
                tags = "---";
            }

            return @$"{card.GetNameWithUrl()} **{card.GetCardRealRarity()}**
*{card.Title ?? Placeholders.Undefined}*


*{card.GetCardParams(true, false, true)}*


**Relacja:** {card.GetAffectionString()}

**Doświadczenie:** {card.ExperienceCount:F}/{card.ExpToUpgrade():F}

**Dostępne ulepszenia:** {card.UpgradesCount}


**W klatce:** {card.InCage.GetYesNo()}

**Aktywna:** {card.Active.GetYesNo()}

**Możliwość wymiany:** {card.IsTradable.GetYesNo()}


**WID:** {card.Id} *({card.CharacterId})*

**Restarty:** {card.RestartCount}

**Pochodzenie:** {card.Source.GetString()}

**Tagi:** {tags}

{card.GetStatusIcons()}
            
";
        }

        public static string GetDescSmall(this Card card)
        {
            var tags = string.Join(" ", card.Tags.Select(x => x.Name));
            var cardSummary = card.GetString(true, true, true, false, true);
            if (card.Tags.Count < 1)
            {
                tags = "---";
            }

            return @$"**[{card.Id}]** *({card.CharacterId})*

{cardSummary}

_{card.Title}_


{card.Dere}

{card.GetAffectionString()}

{card.ExperienceCount:F}/{card.ExpToUpgrade():F} exp


{tags}

{card.GetStatusIcons()}";
        }
    }
}