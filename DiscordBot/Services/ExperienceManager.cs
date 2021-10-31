using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Game.Services;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Services
{
    public class ExperienceManager
    {
        private const double SAVE_AT = 5;

        private Dictionary<ulong, double> _userExperienceMap;

        private Dictionary<ulong, DateTime> _saved;
        private Dictionary<ulong, ulong> _characters;

        private readonly DiscordSocketClient _client;
        private readonly IProducerConsumerCollection<BaseMessage> _blockingPriorityQueue;
        private readonly IImageProcessor _imageProcessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;

        public ExperienceManager(
            DiscordSocketClient client,
            IProducerConsumerCollection<BaseMessage> blockingPriorityQueue,
            IOptionsMonitor<DiscordConfiguration> config,
            IImageProcessor imageProcessor,
            IUserRepository userRepository,
            IUserAnalyticsRepository userAnalyticsRepository,
            ISystemClock systemClock)
        {
            _client = client;
            _blockingPriorityQueue = blockingPriorityQueue;
            _config = config;
            _imageProcessor = imageProcessor;
            _userRepository = userRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _systemClock = systemClock;

            _userExperienceMap = new Dictionary<ulong, double>();
            _saved = new Dictionary<ulong, DateTime>();
            //_messages = new Dictionary<ulong, ulong>();
            //_commands = new Dictionary<ulong, ulong>();
            _characters = new Dictionary<ulong, ulong>();

#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }


       

        private bool CheckLastSave(ulong userId)
        {
            if (!_saved.Any(x => x.Key == userId))
            {
                _saved.Add(userId, _systemClock.UtcNow);
                return false;
            }

            return (_systemClock.UtcNow - _saved[userId].AddMinutes(30)).TotalSeconds > 1;
        }

       

       

    

       
    }
}