using Sanakan.DAL.Models;
using Sanakan.Game.Models;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IEventsService
    {
        EventType RandomizeEvent(ExpeditionCardType expedition, (double, double) duration);
        
        (bool, string) ExecuteEvent(EventType eventType, User user, Card card, string message);
        
        int GetMoreItems(EventType eventType);
    }
}
