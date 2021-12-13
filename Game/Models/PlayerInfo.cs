using Discord;
using Sanakan.DAL.Models;
using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public class PlayerInfo
    {
        public string CustomString { get; set; } = null;

        public List<Card> Cards { get; set; } = new();

        public List<Item> Items { get; set; } = new();

        public ulong DiscordId { get; set; }

        public string Mention { get; set; } = null;

        public bool Accepted { get; set; }

        public User DatabaseUser { get; set; } = null;
    }
}