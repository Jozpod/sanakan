using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
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
        private const double DefaultLevelMultiplier = 0.35;

        private Dictionary<ulong, double> _userExperienceMap;
        private Dictionary<ulong, ulong> _messages;
        private Dictionary<ulong, ulong> _commands;
        private Dictionary<ulong, DateTime> _saved;
        private Dictionary<ulong, ulong> _characters;

        private readonly DiscordSocketClient _client;
        private readonly IProducerConsumerCollection<BaseMessage> _blockingPriorityQueue;
        private readonly IImageProcessor _imageProcessor;
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;

        public ExperienceManager(
            DiscordSocketClient client,
            IProducerConsumerCollection<BaseMessage> blockingPriorityQueue,
            IOptionsMonitor<BotConfiguration> config,
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
            _messages = new Dictionary<ulong, ulong>();
            _commands = new Dictionary<ulong, ulong>();
            _characters = new Dictionary<ulong, ulong>();

#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }

        public static ulong CalculateExpForLevel(ulong level, double levelMultiplier = DefaultLevelMultiplier) 
            => (level <= 0) ? 0ul : (ulong)Convert.ToInt64(Math.Floor(Math.Pow(level / levelMultiplier, 2)) + 1);
        public static ulong CalculateLevel(
            long experience,
            double levelMultiplier = DefaultLevelMultiplier) 
            => (ulong)Convert.ToInt64(Math.Floor(levelMultiplier * Math.Sqrt(experience)));

        public async Task NotifyAboutLevelAsync(
            SocketGuildUser user,
            ISocketMessageChannel channel,
            ulong level)
        {
           
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }

            var user = message.Author as SocketGuildUser;

            if (user == null)
            {
                return;
            }

            if (_config.CurrentValue
                .BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var countMsg = true;
            var calculateExp = true;

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);
            if (config != null)
            {
                var role = user.Guild.GetRole(config.UserRoleId);
                if (role != null)
                {
                    if (!user.Roles.Contains(role))
                    {
                        return;
                    }
                }

                if (config.ChannelsWithoutExp != null)
                {
                    if (config.ChannelsWithoutExp.Any(x => x.Channel == message.Channel.Id))
                        calculateExp = false;
                }

                if (config.IgnoredChannels != null)
                {
                    if (config.IgnoredChannels.Any(x => x.Channel == message.Channel.Id))
                        countMsg = false;
                }
            }

            if (!_messages.Any(x => x.Key == user.Id))
            {
                if (!await _userRepository.ExistsByDiscordIdAsync(user.Id))
                {
                    var databseUser = new User(user.Id, _systemClock.StartOfMonth);
                    _userRepository.Add(databseUser);
                    await _userRepository.SaveChangesAsync();
                }
            }

            if (countMsg)
            {
                CountMessage(user.Id, _config.CurrentValue.IsCommand(message.Content));
            }
            CalculateExpAndCreateTask(user, message, calculateExp);
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

        private void CountMessage(ulong userId, bool isCommand)
        {
            if (!_messages.Any(x => x.Key == userId))
            {
                _messages.Add(userId, 1);
            }
            else
            {
                _messages[userId]++;
            }

            if (!_commands.Any(x => x.Key == userId))
            {
                _commands.Add(userId, isCommand ? 1u : 0u);
            }
            else if (isCommand)
            {
                _commands[userId]++;
            }
        }

        private void CountCharacters(ulong userId, ulong characters)
        {
            if (!_characters.Any(x => x.Key == userId))
                _characters.Add(userId, characters);
            else
                _characters[userId] += characters;
        }

        private void CalculateExpAndCreateTask(SocketGuildUser user, SocketMessage message, bool calculateExp)
        {
            var exp = GetPointsFromMsg(message);
            if (!calculateExp) exp = 0;

            if (!_userExperienceMap.Any(x => x.Key == user.Id))
            {
                _userExperienceMap.Add(message.Author.Id, exp);
                return;
            }

            _userExperienceMap[user.Id] += exp;

            var saved = _userExperienceMap[user.Id];
            if (saved < SAVE_AT && !CheckLastSave(user.Id)) {
                return;
            }

            var fullP = (long) Math.Floor(saved);
            if (fullP < 1)
            {
                return;
            }

            _userExperienceMap[message.Author.Id] -= fullP;
            _saved[user.Id] = _systemClock.UtcNow;

            var task = CreateTask(user, message.Channel, fullP, _messages[user.Id], _commands[user.Id], _characters[user.Id], calculateExp);
            _characters[user.Id] = 0;
            _messages[user.Id] = 0;
            _commands[user.Id] = 0;

            _blockingPriorityQueue.TryAdd(new AddExperienceMessage
            {

            })

            _executor.TryAdd(new Executable("add exp", task), TimeSpan.FromSeconds(1));
        }

        private double GetPointsFromMsg(SocketMessage message)
        {
            int emoteChars = message.Tags.CountEmotesTextLenght();
            int linkChars = message.Content.CountLinkTextLength();
            int nonWhiteSpaceChars = message.Content.Count(c => c != ' ');
            int quotedChars = message.Content.CountQuotedTextLength();
            double charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            CountCharacters(message.Author.Id, (ulong)(charsThatMatters < 1 ? 1 : charsThatMatters));
            return GetExpPointBasedOnCharCount(charsThatMatters);
        }

        private double GetExpPointBasedOnCharCount(double charCount)
        {
            var config = _config.CurrentValue;
            var cpp = config.Exp.CharPerPoint;
            var min = config.Exp.MinPerMessage;
            var max = config.Exp.MaxPerMessage;

            double experience = charCount / cpp;
            if (experience < min)
            {
                return min;
            }

            if (experience > max)
            {
                return max;
            }

            return experience;
        }

      
        //private Task<Task> CreateTask(
        //    SocketGuildUser discordUser,
        //    ISocketMessageChannel channel,
        //    ulong experience,
        //    ulong messages,
        //    ulong commands,
        //    ulong characters,
        //    bool calculateExp)
        //{
        //    return new Task<Task>(async () =>
        //    {
               
        //    });
        //}
    }
}