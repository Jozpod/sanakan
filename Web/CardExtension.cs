using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Sanakan.Api.Models;
using Sanakan.DAL.Models;

namespace Sanakan.Extensions
{
    public static class CardExtension
    {

        public static int GetCardStarType(this Card card)
        {
            var max = card.MaxStarType();
            var maxRestartsPerType = card.GetMaxStarsPerType() * card.GetRestartCntPerStar();
            var type = (card.RestartCnt - 1) / maxRestartsPerType;
            if (type > 0)
            {
                var ths = card.RestartCnt - (maxRestartsPerType + ((type - 1) * maxRestartsPerType));
                if (ths < card.GetRestartCntPerStar()) --type;
            }

            if (type > max) type = max;
            return type;
        }

        public static int GetMaxCardsRestartsOnStarType(this Card card)
        {
            return  card.GetMaxStarsPerType() * card.GetRestartCntPerStar() * card.GetCardStarType();
        }

        public static int GetCardStarCount(this Card card)
        {
            var max = card.GetMaxStarsPerType();
            var starCnt = (card.RestartCnt - card.GetMaxCardsRestartsOnStarType()) / card.GetRestartCntPerStar();
            if (starCnt > max) starCnt = max;
            return starCnt;
        }

      
       
      
        

        public static Api.Models.CardFinalView ToView(this Card c)
            => Api.Models.CardFinalView.ConvertFromRaw(c);

        public static Api.Models.ExpeditionCard ToExpeditionView(this Card c, double karma)
            => Api.Models.ExpeditionCard.ConvertFromRaw(c, karma);

        public static List<Api.Models.ExpeditionCard> ToExpeditionView(this IEnumerable<Card> clist, double karma)
        {
            var list = new List<Api.Models.ExpeditionCard>();
            foreach (var c in clist) list.Add(c.ToExpeditionView(karma));
            return list;
        }

        public static List<CardFinalView> ToView(this IEnumerable<Card> clist)
        {
            var list = new List<Api.Models.CardFinalView>();
            foreach (var c in clist) list.Add(c.ToView());
            return list;
        }
    }
}