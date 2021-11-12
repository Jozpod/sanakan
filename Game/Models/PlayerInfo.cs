using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public class PlayerInfo
    {
        public string CustomString { get; set; }
        public List<Card> Cards { get; set; }
        public List<Item> Items { get; set; }
        public IUser User { get; set; }
        public bool Accepted { get; set; }
        public User DatabaseUser { get; set; }
    }
}