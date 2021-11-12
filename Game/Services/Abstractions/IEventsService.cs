using DiscordBot.Services.PocketWaifu;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IEventsService
    {
        EventType RandomizeEvent(ExpeditionCardType expedition, Tuple<double, double> duration);
        (bool, string) ExecuteEvent(EventType eventType, User user, Card card, string message);
        int GetMoreItems(EventType eventType);
    }
}
