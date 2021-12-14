﻿using Sanakan.Api.Models;
using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class CardExtension
    {
        public static List<ExpeditionCard> ToExpeditionView(this IEnumerable<Card> cards, double karma)
        {
            var list = new List<ExpeditionCard>();
            foreach (var card in cards)
            {
                list.Add(new ExpeditionCard(card, karma));
            }
            return list;
        }

        public static List<CardFinalView> ToView(this IEnumerable<Card> cards)
        {
            var list = new List<CardFinalView>();
            foreach (var card in cards)
            {
                list.Add(new CardFinalView(card));
            }
            return list;
        }
    }
}