using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes the card.
    /// </summary>
    public class CardFinalView
    {
        public CardFinalView()
        {
        }

        public CardFinalView(Card card)
        {
            Id = card.Id;
            IsActive = card.Active;
            IsInCage = card.InCage;
            IsTradable = card.IsTradable;
            IsUnique = card.IsUnique;
            IsUltimate = card.FromFigure;
            ExperienceCount = card.ExperienceCount;
            Affection = card.GetAffectionString();
            UpgradesCount = card.UpgradesCount;
            RestartCount = card.RestartCount;
            Rarity = card.Rarity;
            Dere = card.Dere;
            Defence = card.GetDefenceWithBonus();
            Attack = card.GetAttackWithBonus();
            BaseHealth = card.Health;
            FinalHealth = card.GetHealthWithPenalty();
            Name = card.Name;
            CharacterUrl = card.GetCharacterUrl();
            Source = card.Source.GetString();
            AnimeTitle = card.Title ?? Placeholders.Undefined;
            UltimateQuality = card.Quality;
            CreatedOn = card.CreatedOn;
            CardPower = card.CardPower;
            Value = card.GetThreeStateMarketValue();
            ExperienceCountForNextLevel = card.ExpToUpgrade();
            HasCustomImage = card.CustomImageUrl != null;
            HasCustomBorder = card.CustomBorderUrl != null;
            ImageUrl = $"https://cdn2.shinden.eu/{card.Id}.png";
            IsOnExpedition = card.Expedition != ExpeditionCardType.None;
            SmallImageUrl = $"https://cdn2.shinden.eu/small/{card.Id}.png";
            ProfileImageUrl = $"https://cdn2.shinden.eu/profile/{card.Id}.png";
            Tags = card.Tags.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// The card identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Specifies whether the card is in deck.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Specifies whether the card is in cage.
        /// </summary>
        public bool IsInCage { get; set; }

        /// <summary>
        /// Specifies whether the card is on adventure.
        /// </summary>
        public bool IsOnExpedition { get; set; }

        /// <summary>
        /// Specifies whether the card can be traded.
        /// </summary>
        public bool IsTradable { get; set; }

        /// <summary>
        /// Specifies whether the card is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Specifies whether the card is ultimate card.
        /// </summary>
        public bool IsUltimate { get; set; }

        /// <summary>
        /// Specifies whether the card has custom image.
        /// </summary>
        public bool HasCustomImage { get; set; }

        /// <summary>
        /// Specifies whether the card has custom border.
        /// </summary>
        public bool HasCustomBorder { get; set; }

        /// <summary>
        /// The amount of experience points in a card.
        /// </summary>
        public double ExperienceCount { get; set; }

        /// <summary>
        /// The amount of experience needed for next level.
        /// </summary>
        public double ExperienceCountForNextLevel { get; set; }

        /// <summary>
        /// The rough estimated card power.
        /// </summary>
        public double CardPower { get; set; }

        /// <summary>
        /// The card affection level.
        /// </summary>
        public string Affection { get; set; } = string.Empty;

        /// <summary>
        /// Specifies how many times card can be upgraded.
        /// </summary>
        public int UpgradesCount { get; set; }

        /// <summary>
        /// Specifies how many times card has been restarted.
        /// </summary>
        public int RestartCount { get; set; }

        /// <summary>
        /// The quality of card.
        /// </summary>
        public Rarity Rarity { get; set; }

        /// <summary>
        /// The character of card.
        /// </summary>
        public Dere Dere { get; set; }

        /// <summary>
        /// The defence points.
        /// </summary>
        public int Defence { get; set; }

        /// <summary>
        /// The attack points.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The base health points.
        /// </summary>
        public int BaseHealth { get; set; }

        /// <summary>
        /// The card health points decreased by relation.
        /// </summary>
        public int FinalHealth { get; set; }

        /// <summary>
        /// The full name of the characted.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// The character url.
        /// </summary>
        public string CharacterUrl { get; set; } = null;

        /// <summary>
        /// The card source.
        /// </summary>
        public string Source { get; set; } = null;

        /// <summary>
        /// The anime title.
        /// </summary>
        public string AnimeTitle { get; set; } = null;

        /// <summary>
        /// The card image url.
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// The small image link.
        /// </summary>
        public string SmallImageUrl { get; set; } = null;

        /// <summary>
        /// The card image url which will be shown when set in profile.
        /// </summary>
        public string ProfileImageUrl { get; set; } = null;

        /// <summary>
        /// The market value.
        /// </summary>
        public MarketValue Value { get; set; }

        /// <summary>
        /// The ultimate quality level.
        /// </summary>
        public Quality UltimateQuality { get; set; }

        /// <summary>
        /// The datetime when card was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The tags associated with card.
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}