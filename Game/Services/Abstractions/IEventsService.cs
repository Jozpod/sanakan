using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System.Text;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IEventsService
    {
        EventType RandomizeEvent(ExpeditionCardType expedition, (double, double) duration);

        bool ExecuteEvent(
            EventType eventType,
            User user,
            Card card,
            StringBuilder stringBuilder,
            double totalExperience);

        int GetMoreItems(EventType eventType);
    }
}
