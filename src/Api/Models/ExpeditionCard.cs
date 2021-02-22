﻿#pragma warning disable 1591

using System;
using Sanakan.Database.Models;
using Sanakan.Extensions;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Karta na ekspedycji
    /// </summary>
    public class ExpeditionCard
    {
        /// <summary>
        /// Moment wyruszenia na ekspedycję
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Rodzaj wyprawy
        /// </summary>
        public string Expedition { get; set; }
        /// <summary>
        /// Karta
        /// </summary>
        public CardFinalView Card { get; set; }

        public static ExpeditionCard ConvertFromRaw(Card card)
        {
            return new ExpeditionCard
            {
                Card = card.ToView(),
                StartTime = card.ExpeditionDate,
                Expedition = card.Expedition.GetName()
            };
        }
    }
}