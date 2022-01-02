﻿using Sanakan.DAL.Models;
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

        public static IQueryable<Card> QueryOrderBy(OrderType type, IQueryable<Card> query)
        {
            switch (type)
            {
                case OrderType.Atack:
                    return query.OrderBy(x => x.Attack + x.AttackBonus + (x.RestartCount * 2d));
                case OrderType.AtackDes:
                    return query.OrderByDescending(x => x.Attack + x.AttackBonus + (x.RestartCount * 2d));
                case OrderType.Exp:
                    return query.OrderBy(x => x.ExperienceCount);
                case OrderType.ExpDes:
                    return query.OrderByDescending(x => x.ExperienceCount);
                case OrderType.Dere:
                    return query.OrderBy(x => x.Dere);
                case OrderType.DereDes:
                    return query.OrderByDescending(x => x.Dere);
                case OrderType.Defence:
                    return query.OrderBy(x => x.Defence + x.DefenceBonus + x.RestartCount);
                case OrderType.DefenceDes:
                    return query.OrderByDescending(x => x.Defence + x.DefenceBonus + x.RestartCount);
                case OrderType.Health:
                    return query.OrderBy(x => x.Health + ((x.Health * (x.Affection * 5d / 100d)) + x.HealthBonus));
                case OrderType.HealthDes:
                    return query.OrderByDescending(x => x.Health + ((x.Health * (x.Affection * 5d / 100d)) + x.HealthBonus));
                case OrderType.HealthBase:
                    return query.OrderBy(x => x.Health);
                case OrderType.HealthBaseDes:
                    return query.OrderByDescending(x => x.Health);
                case OrderType.CardPower:
                    return query.OrderBy(x => x.CardPower);
                case OrderType.CardPowerDes:
                    return query.OrderByDescending(x => x.CardPower);
                case OrderType.Relation:
                    return query.OrderBy(x => x.Affection);
                case OrderType.RelationDes:
                    return query.OrderByDescending(x => x.Affection);
                case OrderType.Title:
                    return query.OrderBy(x => x.Title);
                case OrderType.TitleDes:
                    return query.OrderByDescending(x => x.Title);
                case OrderType.RarityDes:
                    return query.OrderBy(x => x.Rarity).ThenByDescending(x => x.Quality);
                case OrderType.Rarity:
                    return query.OrderByDescending(x => x.Rarity).ThenBy(x => x.Quality);
                case OrderType.Name:
                    return query.OrderBy(x => x.Name);
                case OrderType.NameDes:
                    return query.OrderByDescending(x => x.Name);
                case OrderType.Picture:
                    return query.OrderBy(x => (x.CustomImageUrl == null ? (x.ImageUrl == null ? 0 : 1) : 2));
                case OrderType.PictureDes:
                    return query.OrderByDescending(x => (x.CustomImageUrl == null ? (x.ImageUrl == null ? 0 : 1) : 2));
                case OrderType.IdDes:
                    return query.OrderByDescending(x => x.Id);

                default:
                case OrderType.Id:
                    return query;
            }
        }
    }
}