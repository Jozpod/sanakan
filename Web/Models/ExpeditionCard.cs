using System;
using Sanakan.DAL.Models;
using Sanakan.Extensions;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Describes expedition card.
    /// </summary>
    public class ExpeditionCard
    {
        public ExpeditionCard(Card? card, double karma)
        {
            Card = card == null ? null : new CardFinalView(card);
            StartedOn = card.ExpeditionDate;
            ExpeditionType = card.Expedition.GetName();
            MaxTime = card.CalculateMaxTimeOnExpeditionInMinutes(karma);
        }

        /// <summary>
        /// The datetime when expedition started.
        /// </summary>
        public DateTime StartedOn { get; set; }

        /// <summary>
        /// The type of expedition.
        /// </summary>
        public string ExpeditionType { get; set; }
        
        /// <summary>
        /// The amount of time in minutes after which card won't get any bonus points.
        /// </summary>
        public double MaxTime { get; set; }

        /// <summary>
        /// The card related to the expedition.
        /// </summary>
        public CardFinalView? Card { get; set; }
    }
}