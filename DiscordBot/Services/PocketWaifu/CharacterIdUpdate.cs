using System;
using System.Collections.Generic;

namespace Sanakan.Services.PocketWaifu
{
    public class CharacterIdUpdate
    {
        public CharacterIdUpdate()
        {
            EventEnabled = false;
            Ids = new List<ulong>();
            EventIds = new List<ulong>();
            LastUpdate = DateTime.MinValue;
        }

       

        public void SetEventIds(List<ulong> ids)
            => EventIds = ids;

        public void Update(List<ulong> ids)
        {
            LastUpdate = _systemClock.UtcNow;
            Ids = ids;
        }

        public bool IsNeedForUpdate()
            => (_systemClock.UtcNow - LastUpdate).TotalDays >= 1;

        public bool EventEnabled { get; set; }
        public List<ulong> EventIds { get; private set; }

        public List<ulong> Ids { get; private set; }
        public DateTime LastUpdate { get; private set; }
    }
}