using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Services.PocketWaifu.Abstractions
{
    public interface IWaifuService
    {
        bool GetEventSate() => CharId.EventEnabled;

        void SetEventState(bool state) => CharId.EventEnabled = state;

        void SetEventIds(List<ulong> ids) => CharId.SetEventIds(ids);

        List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag);
    }
}
