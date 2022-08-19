using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sanakan.DAL.Repositories
{
    /// <summary>
    /// Card filter.
    /// </summary>
    public class CardsQueryFilter
    {
        /// <summary>
        /// Order by criteria.
        /// </summary>
        public OrderType OrderBy { get; set; }

        /// <summary>
        /// Text to search.
        /// </summary>
        [StringLength(50)]
        public string SearchText { get; set; } = null;

        /// <summary>
        /// Tags to include in query.
        /// </summary>
        public List<string> IncludeTags { get; set; } = new();

        /// <summary>
        /// Tags to exclude in query.
        /// </summary>
        public List<string> ExcludeTags { get; set; } = new();

        /// <summary>
        /// Descibes the method by which tags are filtered.
        /// </summary>
        public FilterTagsMethodType FilterTagsMethod { get; set; }

        public static IQueryable<Card> QueryOrderBy(OrderType type, IQueryable<Card> query) => type switch
        {
            OrderType.Atack => query.OrderBy(x => x.Attack + x.AttackBonus + (x.RestartCount * 2d)),
            OrderType.AtackDes => query.OrderByDescending(x => x.Attack + x.AttackBonus + (x.RestartCount * 2d)),
            OrderType.Exp => query.OrderBy(x => x.ExperienceCount),
            OrderType.ExpDes => query.OrderByDescending(x => x.ExperienceCount),
            OrderType.Dere => query.OrderBy(x => x.Dere),
            OrderType.DereDes => query.OrderByDescending(x => x.Dere),
            OrderType.Defence => query.OrderBy(x => x.Defence + x.DefenceBonus + x.RestartCount),
            OrderType.DefenceDes => query.OrderByDescending(x => x.Defence + x.DefenceBonus + x.RestartCount),
            OrderType.Health => query.OrderBy(x => x.Health + ((x.Health * (x.Affection * 5d / 100d)) + x.HealthBonus)),
            OrderType.HealthDes => query.OrderByDescending(x => x.Health + ((x.Health * (x.Affection * 5d / 100d)) + x.HealthBonus)),
            OrderType.HealthBase => query.OrderBy(x => x.Health),
            OrderType.HealthBaseDes => query.OrderByDescending(x => x.Health),
            OrderType.CardPower => query.OrderBy(x => x.CardPower),
            OrderType.CardPowerDes => query.OrderByDescending(x => x.CardPower),
            OrderType.Relation => query.OrderBy(x => x.Affection),
            OrderType.RelationDes => query.OrderByDescending(x => x.Affection),
            OrderType.Title => query.OrderBy(x => x.Title),
            OrderType.TitleDes => query.OrderByDescending(x => x.Title),
            OrderType.RarityDes => query.OrderBy(x => x.Rarity).ThenByDescending(x => x.Quality),
            OrderType.Rarity => query.OrderByDescending(x => x.Rarity).ThenBy(x => x.Quality),
            OrderType.Name => query.OrderBy(x => x.Name),
            OrderType.NameDes => query.OrderByDescending(x => x.Name),
            OrderType.Picture => query.OrderBy(x => (x.CustomImageUrl == null ? (x.ImageUrl == null ? 0 : 1) : 2)),
            OrderType.PictureDes => query.OrderByDescending(x => (x.CustomImageUrl == null ? (x.ImageUrl == null ? 0 : 1) : 2)),
            OrderType.IdDes => query.OrderByDescending(x => x.Id),
            _ => query,
        };
    }
}